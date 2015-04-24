using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Transactions;
using Moq;
using NUnit.Framework;
using PipelineFramework.Tests.TestData.Discovery;
using PipelineFramework.Tests.TestData.Discovery.TestObjects;
using PipelinePlusPlus.Core.Context;
using PipelinePlusPlus.Core.Discovery;
using PipelinePlusPlus.Core.DynamicConfig;
using PipelinePlusPlus.Core.EventArgs;
using PipelinePlusPlus.Core.Exceptions;
using PipelinePlusPlus.Core.Modules;
using PipelinePlusPlus.Core.Modules.Mananger;
using PipelinePlusPlus.Core.Steps;

namespace PipelineFramework.Tests
{
    [TestFixture]
    // resetting delegates is a pain in the arse. 
    [SuppressMessage("ReSharper", "DelegateSubtraction")]
    public class PipelineDiscoveryTests
    {
        private Mock<IDynamicModuleConfig> _mockDynamicConfig;
        private Mock<PipelineDynamicModuleConfig> _mockDynamicConfigResult;
        private Mock<IPipelineModuleManager> _mockModuleMananger;
        private EventHandler<PipelineModuleInitializedEventArgs> _moduleInitializedHandler;
        private EventHandler<PipelineModuleInitializingEventArgs> _moduleInitializingHandler;

        public void Setup<TPipeline, TContext>() where TContext : PipelineStepContext where TPipeline : PipelineSteps
        {
            _mockModuleMananger = new Mock<IPipelineModuleManager>(MockBehavior.Strict);

            //module mananger does nothing.
            _mockModuleMananger.Setup(m => m.RegisterDynamicModules<TPipeline, TContext>(It.IsAny<TPipeline>(), It.IsAny<PipelineDynamicModuleConfig>(), It.IsAny<EventHandler<PipelineModuleInitializingEventArgs>>(), It.IsAny<EventHandler<PipelineModuleInitializedEventArgs>>()));

            _mockModuleMananger.Setup(m => m.RegisterModules(It.IsAny<TPipeline>(), It.IsAny<IEnumerable<PipelineModule<TPipeline, TContext>>>(), It.IsAny<EventHandler<PipelineModuleInitializingEventArgs>>(), It.IsAny<EventHandler<PipelineModuleInitializedEventArgs>>()));

            // mock result for dynamic config
            _mockDynamicConfigResult = new Mock<PipelineDynamicModuleConfig>(MockBehavior.Strict, TestUtils.PipelineNameForTest);

            // mock dynamic config returns mock result...
            _mockDynamicConfig = new Mock<IDynamicModuleConfig>(MockBehavior.Strict);

            _mockDynamicConfig.Setup(m => m.GetConfig(It.IsAny<String>(), It.IsAny<Configuration>()))
                .Returns(_mockDynamicConfigResult.Object);

            // reset event handlers. 
            _moduleInitializingHandler += (sender, args) => { };
            _moduleInitializedHandler += (sender, args) => { };
            if (_moduleInitializingHandler != null)
            {
                var initliaizing = _moduleInitializingHandler.GetInvocationList();

                foreach (var del in initliaizing)
                {
                    _moduleInitializingHandler -= (EventHandler<PipelineModuleInitializingEventArgs>) del;
                }
            }

            if (_moduleInitializedHandler != null)
            {
                var initialized = _moduleInitializedHandler.GetInvocationList();
                foreach (var del in initialized)
                {
                    _moduleInitializedHandler -= (EventHandler<PipelineModuleInitializedEventArgs>) del;
                }
            }
        }

        private IPipelineDiscovery<TPipeline, DiscoveryTestStepContext> CreateSut<TPipeline>(TPipeline steps, Action postSetup = null) where TPipeline : PipelineSteps
        {
            Setup<TPipeline, DiscoveryTestStepContext>();
            if (postSetup != null)
            {
                postSetup();
            }
            return new PipelineDiscovery<TPipeline, DiscoveryTestStepContext>(_mockDynamicConfig.Object, _mockModuleMananger.Object, steps, _moduleInitializedHandler, _moduleInitializingHandler);
        }

        [Test]
        public void WhenCallingResolvePipelineWithValidSteps_ShouldProduceCorrectDefinition()
        {
            // arrange
            var config = TestUtils.GenerateConfig(TestUtils.PipelineNameForTest);

            var steps = new ThreeStepsWithAttributes();

            var sut = CreateSut(steps);

            // act
            PipelineDefinition<DiscoveryTestStepContext> result = null;

            Assert.DoesNotThrow(() => { result = sut.ResolvePipeline(steps.CreateEmptyModuleCollection(), config); });

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(steps.TestStep1, Is.Not.Null);
            Assert.That(steps.TestStep2, Is.Not.Null);
            Assert.That(steps.TestStep3, Is.Not.Null);
            Assert.That(result.PipelineName, Is.EqualTo(steps.PipelineName));
            Assert.That(result.PipelineScopeOption, Is.EqualTo(TransactionScopeOption.Suppress));
            Assert.That(result.Actions.Count, Is.EqualTo(3));

            var castActions = result.Actions.Cast<PipelineStepDefinintion<DiscoveryTestStepContext>>()
                .ToArray();

            Assert.That(castActions, Is.Not.Empty);
            Assert.That(castActions.Count(), Is.EqualTo(result.Actions.Count));

            // happy path, if the first item is mapped correctly i'm gonna assume the rest are. 
            // other tests will cover variances in the attribute usage on steps
            var step1 = castActions[0];

            Assert.That(step1.Attr, Is.Not.Null);
            Assert.That(step1.Attr.Order, Is.EqualTo(0));
            Assert.That(step1.Attr.TransactionScopeOption, Is.EqualTo(TransactionScopeOption.Suppress));

            // is the instance i hold in my def the same ref as the instance on the steps.  :)
            Assert.That(step1.Step, Is.SameAs(steps.TestStep1));
            Assert.That(step1.StepName, Is.EqualTo("TestStep1"));

            // assert calls to module manager
            _mockModuleMananger.VerifyRegisterModulesCall(m => m != null && !m.Any(), _moduleInitializingHandler, _moduleInitializedHandler, steps);
            _mockModuleMananger.VerifyRegisterDynamicModulesCall(_mockDynamicConfigResult, _moduleInitializingHandler, _moduleInitializedHandler, steps);

            // assert call to dynamic config
            _mockDynamicConfig.VerifyGetConfigCall(config);
        }

        [Test]
        public void WhenCallingResolvePipelineWithValidStepsWhereOneIsAlreadyInstantiated_ShouldInstantiateOtherSteps()
        {
            // arrange
            var config = TestUtils.GenerateConfig(TestUtils.PipelineNameForTest);

            var steps = new ThreeStepsWithAttributes
            {
                TestStep3 = new PipelineStep<DiscoveryTestStepContext>()
            };

            var step3 = steps.TestStep3;

            var sut = CreateSut(steps);

            // act
            PipelineDefinition<DiscoveryTestStepContext> result = null;

            Assert.DoesNotThrow(() => { result = sut.ResolvePipeline(steps.CreateEmptyModuleCollection(), config); });

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(steps.TestStep1, Is.Not.Null);
            Assert.That(steps.TestStep2, Is.Not.Null);

            // are all the references all the same!
            Assert.That(step3, Is.SameAs(steps.TestStep3));
            Assert.That(step3, Is.SameAs(((PipelineStepDefinintion<DiscoveryTestStepContext>) result.Actions.Last()).Step));

            // assert calls to module manager
            _mockModuleMananger.VerifyRegisterModulesCall(m => m != null && !m.Any(), _moduleInitializingHandler, _moduleInitializedHandler, steps);
            _mockModuleMananger.VerifyRegisterDynamicModulesCall(_mockDynamicConfigResult, _moduleInitializingHandler, _moduleInitializedHandler, steps);

            // assert call to dynamic config
            _mockDynamicConfig.VerifyGetConfigCall(config);
        }

        [Test]
        public void WhenCallingResolvePipelineWithAStepWithAttributeScopeOptionRequired_ShouldDefineCorrectTranScope()
        {
            // arrange
            var config = TestUtils.GenerateConfig(TestUtils.PipelineNameForTest);

            var steps = new ThreeStepsWithRequiredTranScope();

            var sut = CreateSut(steps);

            // act
            PipelineDefinition<DiscoveryTestStepContext> result = null;

            Assert.DoesNotThrow(() => { result = sut.ResolvePipeline(new List<PipelineModule<ThreeStepsWithRequiredTranScope, DiscoveryTestStepContext>>(), config); });

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.PipelineScopeOption, Is.EqualTo(TransactionScopeOption.Required));

            // assert calls to module manager
            _mockModuleMananger.VerifyRegisterModulesCall(m => m != null && !m.Any(), _moduleInitializingHandler, _moduleInitializedHandler, steps);
            _mockModuleMananger.VerifyRegisterDynamicModulesCall(_mockDynamicConfigResult, _moduleInitializingHandler, _moduleInitializedHandler, steps);

            // assert call to dynamic config
            _mockDynamicConfig.VerifyGetConfigCall(steps.PipelineName, config);
        }

        [Test]
        public void WhenCallingResolvePipelineWithAStepWithAttributeScopeOptionRequireNew_ShouldDefineCorrectTranScope()
        {
            // arrange
            var config = TestUtils.GenerateConfig(TestUtils.PipelineNameForTest);

            var steps = new ThreeStepsWithRequiresNewTranScope();

            var sut = CreateSut(steps);

            // act
            PipelineDefinition<DiscoveryTestStepContext> result = null;

            Assert.DoesNotThrow(() => { result = sut.ResolvePipeline(new List<PipelineModule<ThreeStepsWithRequiresNewTranScope, DiscoveryTestStepContext>>(), config); });

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.PipelineScopeOption, Is.EqualTo(TransactionScopeOption.RequiresNew));

            // assert calls to module manager
            _mockModuleMananger.VerifyRegisterModulesCall(m => m != null && !m.Any(), _moduleInitializingHandler, _moduleInitializedHandler, steps);
            _mockModuleMananger.VerifyRegisterDynamicModulesCall(_mockDynamicConfigResult, _moduleInitializingHandler, _moduleInitializedHandler, steps);

            // assert call to dynamic config
            _mockDynamicConfig.VerifyGetConfigCall(steps.PipelineName, config);
        }

        [Test]
        public void WhenCallingResolvePipelineWithStepsForWrongContext_ShouldThrowException()
        {
            // arrange
            var config = TestUtils.GenerateConfig(TestUtils.PipelineNameForTest);

            var steps = new ThreeStepsForWrongContext();

            var sut = CreateSut(steps);

            // act
            var ex = Assert.Throws<PipelineDicoveryException>(() => { var result = sut.ResolvePipeline(steps.CreateEmptyModuleCollection(), config); });

            // assert
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Is.EqualTo(string.Format("No properties found on the Pipeline Definition '{0}'. Discovery Aborted", steps.PipelineName)));

            // assert lack of calls to other components
            _mockModuleMananger.VerifyRegisterModulesNotCalled(steps);
            _mockModuleMananger.VerifyRegisterDynamicModulesNotCalled(steps);

            _mockDynamicConfig.VerifyGetConfigNotCalled();
        }

        [Test]
        public void WhenCallingResolvePipelineWithStepsThatHaveInvalidSequenceOrder_ShouldThrowException()
        {
            // arrange
            var config = TestUtils.GenerateConfig(TestUtils.PipelineNameForTest);

            var steps = new ThreeStepsWithInvalidOrder();

            var sut = CreateSut(steps);

            // act
            var ex = Assert.Throws<PipelineDicoveryException>(() => { var result = sut.ResolvePipeline(steps.CreateEmptyModuleCollection(), config); });

            // assert
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Is.EqualTo("The order value in the PipelineStepAttribute has duplicates. Please review your steps and ensure that the SequenceOrder value is set correctly and not duplicated per task."));

            // assert lack of calls to other components
            _mockModuleMananger.VerifyRegisterModulesNotCalled(steps);
            _mockModuleMananger.VerifyRegisterDynamicModulesNotCalled(steps);

            _mockDynamicConfig.VerifyGetConfigNotCalled();
        }

        [Test]
        public void WhenCallingResolvePipelineWithStepsMissingAnAttribute_ShouldThrowException()
        {
            // arrange
            var config = TestUtils.GenerateConfig(TestUtils.PipelineNameForTest);

            var steps = new ThreeStepsWithMissingAttribute();

            var sut = CreateSut(steps);

            // act
            var ex = Assert.Throws<PipelineDicoveryException>(() => { var result = sut.ResolvePipeline(new List<PipelineModule<ThreeStepsWithMissingAttribute, DiscoveryTestStepContext>>(), config); });

            // assert
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Is.EqualTo(string.Format("Step '{0}' has no PipelineStepAttribute defined. Please review the pipeline definition '{1}'", "TestStep3", steps.PipelineName)));

            // assert lack of calls to other components
            _mockModuleMananger.VerifyRegisterModulesNotCalled(steps);
            _mockModuleMananger.VerifyRegisterDynamicModulesNotCalled(steps);

            _mockDynamicConfig.VerifyGetConfigNotCalled();
        }

        [Test]
        public void WhenCallingResolvePipelineRegisterModulesThrowsError_ErrorShouldBubble()
        {
            // arrange

            var expectedException = new NotImplementedException("Custom Exception");

            var config = TestUtils.GenerateConfig(TestUtils.PipelineNameForTest);

            var steps = new ThreeStepsWithAttributes();

            var sut = CreateSut(steps, () =>
            {
                _mockModuleMananger.Setup(m => m.RegisterModules(It.IsAny<ThreeStepsWithAttributes>(), It.IsAny<IEnumerable<PipelineModule<ThreeStepsWithAttributes, DiscoveryTestStepContext>>>(), It.IsAny<EventHandler<PipelineModuleInitializingEventArgs>>(), It.IsAny<EventHandler<PipelineModuleInitializedEventArgs>>()))
                    .Throws(expectedException);
            });

            // act
            var ex = Assert.Throws<NotImplementedException>(() => { var result = sut.ResolvePipeline(steps.CreateEmptyModuleCollection(), config); });

            // assert
            Assert.That(ex, Is.EqualTo(expectedException));
        }

        [Test]
        public void WhenCallingResolvePipelineRegisterDynamicModulesThrowsError_ErrorShouldBubble()
        {
            // arrange

            var expectedException = new NotImplementedException("Custom Exception");

            var config = TestUtils.GenerateConfig(TestUtils.PipelineNameForTest);

            var steps = new ThreeStepsWithAttributes();

            var sut = CreateSut(steps, () =>
            {
                _mockModuleMananger.Setup(m => m.RegisterDynamicModules<ThreeStepsWithAttributes, DiscoveryTestStepContext>(It.IsAny<ThreeStepsWithAttributes>(), It.IsAny<PipelineDynamicModuleConfig>(), It.IsAny<EventHandler<PipelineModuleInitializingEventArgs>>(), It.IsAny<EventHandler<PipelineModuleInitializedEventArgs>>()))
                    .Throws(expectedException);
            });

            // act
            var ex = Assert.Throws<NotImplementedException>(() => { var result = sut.ResolvePipeline(steps.CreateEmptyModuleCollection(), config); });

            // assert
            Assert.That(ex, Is.EqualTo(expectedException));
        }

        [Test]
        public void WhenCallingResolvePipelineGetConfigThrowsError_ErrorShouldBubble()
        {
            // arrange

            var expectedException = new NotImplementedException("Custom Exception");

            var config = TestUtils.GenerateConfig(TestUtils.PipelineNameForTest);

            var steps = new ThreeStepsWithAttributes();

            var sut = CreateSut(steps, () =>
            {
                _mockDynamicConfig.Setup(m => m.GetConfig(It.IsAny<String>(), It.IsAny<Configuration>()))
                    .Throws(expectedException);
            });

            // act
            var ex = Assert.Throws<NotImplementedException>(() => { var result = sut.ResolvePipeline(steps.CreateEmptyModuleCollection(), config); });

            // assert
            Assert.That(ex, Is.EqualTo(expectedException));
        }

        [Test]
        public void WhenCallingResolvePipelineWithInitializingInitializedEvents_ShouldPassHandlersToModuleManager()
        {
            // arrange
            var config = TestUtils.GenerateConfig(TestUtils.PipelineNameForTest);

            var steps = new ThreeStepsWithRequiresNewTranScope();

            var sut = CreateSut(steps, () =>
            {
                // ncrunch code coverage disabled as these are jsut passed on, the handler is not executed by the sut 
                // ncrunch: no coverage start
                _moduleInitializedHandler += (sender, args) =>
                {
                    var a = 1;
                    a++;
                };

                _moduleInitializingHandler += (sender, args) =>
                {
                    var a = 1;
                    a++;
                };
                // ncrunch: no coverage end
            });

            // act
            PipelineDefinition<DiscoveryTestStepContext> result = null;

            Assert.DoesNotThrow(() => { result = sut.ResolvePipeline(new List<PipelineModule<ThreeStepsWithRequiresNewTranScope, DiscoveryTestStepContext>>(), config); });

            // assert
            Assert.That(result, Is.Not.Null);

            // assert calls to module manager
            _mockModuleMananger.VerifyRegisterModulesCall(m => m != null && !m.Any(), _moduleInitializingHandler, _moduleInitializedHandler, steps);
            _mockModuleMananger.VerifyRegisterDynamicModulesCall(_mockDynamicConfigResult, _moduleInitializingHandler, _moduleInitializedHandler, steps);

            // assert call to dynamic config
            _mockDynamicConfig.VerifyGetConfigCall(steps.PipelineName, config);
        }

    }
}