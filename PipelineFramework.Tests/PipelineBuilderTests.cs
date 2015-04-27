using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Transactions;
using Moq;
using NUnit.Framework;
using PipelineFramework.Tests.TestData.Builder;
using PipelinePlusPlus.Builder;
using PipelinePlusPlus.Core;
using PipelinePlusPlus.Core.Discovery;
using PipelinePlusPlus.Core.EventArgs;
using PipelinePlusPlus.Core.Exceptions;
using PipelinePlusPlus.Core.Modules;
using PipelinePlusPlus.Core.Steps;

namespace PipelineFramework.Tests
{
    [TestFixture]
    public class PipelineBuilderTests
    {
        private Mock<IPipelineDiscovery<TestPipeline, TestPipelineStepContext>> _mockDiscovery;
        private Mock<IDiscoveryFactory> _mockDiscoveryFactory;

        public void Setup()
        {
            _mockDiscovery = new Mock<IPipelineDiscovery<TestPipeline, TestPipelineStepContext>>(MockBehavior.Strict);

            _mockDiscoveryFactory = new Mock<IDiscoveryFactory>(MockBehavior.Strict);

            _mockDiscoveryFactory.Setup(m => m.GetDiscovery<TestPipeline, TestPipelineStepContext>(
                It.IsAny<TestPipeline>(),
                It.IsAny<EventHandler<PipelineModuleInitializedEventArgs>>(),
                It.IsAny<EventHandler<PipelineModuleInitializingEventArgs>>()))
                .Returns(() => _mockDiscovery.Object);
        }

        public PipelineBuilder<TestPipeline, TestPipelineStepContext> CreateSut(PipelineDefinition<TestPipelineStepContext> def, Action postSetup = null)
        {
            Setup();

            _mockDiscovery.Setup(m => m.ResolvePipeline(It.IsAny<IEnumerable<PipelineModule<TestPipeline, TestPipelineStepContext>>>(), It.IsAny<Configuration>())).Returns(def);
            // ncrunch: no coverage start
            if (postSetup != null)
                postSetup();
            // ncrunch: no coverage end

            return new PipelineBuilder<TestPipeline, TestPipelineStepContext>(_mockDiscoveryFactory.Object);
        }

        // when using builder should jsut assert the internal state of the builder. 

        // when using builder.make() should jsut assert that an execution context has been attached to the underlying pipeline. 

        // tests for the execution context will cover the functionality of that. 

        [Test]
        public void WhenUsingBuilderAddingOnPipelineStageExectueHandler_OnPipelineStageExectueHandlerShouldBeUpdated()
        {
            // A NOTE ABOUT HANDLERS. Fuck handlers initializing to null. That's why the counts are +1 than you'd think.

            // Arrange
            var sut = CreateSut(It.IsAny<PipelineDefinition<TestPipelineStepContext>>());

            // the current count of handlers in the builder,
            // must check for null. 
            var initialHandlerCount = sut.PipelineStageExecutingHandler == null ? 0 : sut.PipelineStageExecutingHandler.GetInvocationList().Count();

            // Act

            // try both overloads
            sut.OnPipeLineStageExectue(args => { });
            sut.OnPipeLineStageExectue((sender, args) => { });


            // Assert

            Assert.That(sut.PipelineStageExecutingHandler, Is.Not.Null);
            var finalHandlerCount = sut.PipelineStageExecutingHandler.GetInvocationList().Count();

            Assert.That(initialHandlerCount, Is.Not.EqualTo(finalHandlerCount));
            Assert.That(finalHandlerCount, Is.EqualTo(3));
        }

        [Test]
        public void WhenUsingBuilderAddingOnPipelineStageExectuedHandler_OnPipelineStageExectuedHandlerShouldBeUpdated()
        {
            // A NOTE ABOUT HANDLERS. Fuck handlers initializing to null. That's why the counts are +1 than you'd think.

            // Arrange
            var sut = CreateSut(It.IsAny<PipelineDefinition<TestPipelineStepContext>>());

            // the current count of handlers in the builder,
            // must check for null. 
            var initialHandlerCount = sut.PipelineStageExecutedHandler == null ? 0 : sut.PipelineStageExecutedHandler.GetInvocationList().Count();

            // Act

            // try both overloads
            sut.OnPipeLineStageExectued(args => { });
            sut.OnPipeLineStageExectued((sender, args) => { });


            // Assert
            Assert.That(sut.PipelineStageExecutedHandler, Is.Not.Null);
            var finalHandlerCount = sut.PipelineStageExecutedHandler.GetInvocationList().Count();

            Assert.That(initialHandlerCount, Is.Not.EqualTo(finalHandlerCount));
            Assert.That(finalHandlerCount, Is.EqualTo(3));
        }

        [Test]
        public void WhenUsingBuilderAddingOnModuleInitializingHandler_OnModuleInitializingHandlerShouldBeUpdated()
        {
            // A NOTE ABOUT HANDLERS. Fuck handlers initializing to null. That's why the counts are +1 than you'd think.

            // Arrange
            var sut = CreateSut(It.IsAny<PipelineDefinition<TestPipelineStepContext>>());

            // the current count of handlers in the builder,
            // must check for null. 
            var initialHandlerCount = sut.ModuleInitializingHandler == null ? 0 : sut.ModuleInitializingHandler.GetInvocationList().Count();

            // Act

            // try both overloads
            sut.OnModuleInitialize(args => { });
            sut.OnModuleInitialize((sender, args) => { });


            // Assert
            Assert.That(sut.ModuleInitializingHandler, Is.Not.Null);
            var finalHandlerCount = sut.ModuleInitializingHandler.GetInvocationList().Count();

            Assert.That(initialHandlerCount, Is.Not.EqualTo(finalHandlerCount));
            Assert.That(finalHandlerCount, Is.EqualTo(3));
        }

        [Test]
        public void WhenUsingBuilderAddingOnModuleInitializedHandler_OnModuleInitializedHandlerShouldBeUpdated()
        {
            // A NOTE ABOUT HANDLERS. Fuck handlers initializing to null. That's why the counts are +1 than you'd think.

            // Arrange
            var sut = CreateSut(It.IsAny<PipelineDefinition<TestPipelineStepContext>>());

            // the current count of handlers in the builder,
            // must check for null. 
            var initialHandlerCount = sut.ModuleInitializedHandler == null ? 0 : sut.ModuleInitializedHandler.GetInvocationList().Count();

            // Act

            // try both overloads
            sut.OnModuleInitialized(args => { });
            sut.OnModuleInitialized((sender, args) => { });


            // Assert
            Assert.That(sut.ModuleInitializedHandler, Is.Not.Null);
            var finalHandlerCount = sut.ModuleInitializedHandler.GetInvocationList().Count();

            Assert.That(initialHandlerCount, Is.Not.EqualTo(finalHandlerCount));
            Assert.That(finalHandlerCount, Is.EqualTo(3));
        }

        [Test]
        public void WhenUsingBuilderAddingOnErrorHandler_OnErrorHandlerShouldBeUpdated()
        {
            // Arrange
            var sut = CreateSut(It.IsAny<PipelineDefinition<TestPipelineStepContext>>());

            // the current count of handlers in the builder,
            // must check for null. 
            var initialHandlerCount = sut.OnError.GetInvocationList().Count();

            // Act
            sut.OnPipelineError(e => true);


            // Assert
            Assert.That(sut.OnError, Is.Not.Null);
            var finalHandlerCount = sut.OnError.GetInvocationList().Count();

            Assert.That(initialHandlerCount, Is.EqualTo(finalHandlerCount));
            Assert.That(finalHandlerCount, Is.EqualTo(1));
        }

        [Test]
        public void WhenUsingBuilderAddingMultipeOnErrorHandler_ShouldOnlyHaveLastOnErrorHandlerEntered()
        {
            // Arrange
            var sut = CreateSut(It.IsAny<PipelineDefinition<TestPipelineStepContext>>());

            // the current count of handlers in the builder,
            // must check for null. 
            var initialHandlerCount = sut.OnError.GetInvocationList().Count();

            Func<PipelineException, bool> firstHandler = e => true;
            Func<PipelineException, bool> secondHandler = e => false;

            // Act
            sut.OnPipelineError(firstHandler);
            sut.OnPipelineError(secondHandler);


            // Assert
            Assert.That(sut.OnError, Is.Not.Null);
            var finalHandlerCount = sut.OnError.GetInvocationList().Count();

            Assert.That(initialHandlerCount, Is.EqualTo(finalHandlerCount));
            Assert.That(finalHandlerCount, Is.EqualTo(1));
            Assert.That(sut.OnError, Is.Not.EqualTo(firstHandler));
            Assert.That(sut.OnError, Is.EqualTo(secondHandler));
        }

        [Test]
        public void WhenUsingBuilderToRegisterModuleGeneric_ShouldHaveCorrectNumberOfModules()
        {
            // Arrange
            var sut = CreateSut(It.IsAny<PipelineDefinition<TestPipelineStepContext>>());

            // the current count of handlers in the builder,
            // must check for null. 
            var initialModuleCount = sut.Modules.Count;

            // Act
            sut.RegisterModule<ParamlessModule>();

            // Assert
            Assert.That(sut.Modules, Is.Not.Null);
            var finalModuleCount = sut.Modules.Count;

            Assert.That(initialModuleCount, Is.Not.EqualTo(finalModuleCount));
            Assert.That(finalModuleCount, Is.EqualTo(1));
            // check that module was created and added to list!
            Assert.That(sut.Modules.First(), Is.Not.Null);
        }

        [Test]
        public void WhenUsingBuilderToRegisterModule_ShouldHaveCorrectNumberOfModules()
        {
            // Arrange
            var sut = CreateSut(It.IsAny<PipelineDefinition<TestPipelineStepContext>>());

            // the current count of modules in the builder,
            var initialModuleCount = sut.Modules.Count;

            // Act
            var theModule = new ParamlessModule();

            sut.RegisterModule(theModule);

            // Assert
            Assert.That(sut.Modules, Is.Not.Null);
            var finalModuleCount = sut.Modules.Count;

            Assert.That(initialModuleCount, Is.Not.EqualTo(finalModuleCount));
            Assert.That(finalModuleCount, Is.EqualTo(1));
            // check module is same instance we assinged in test. 
            Assert.That(sut.Modules.First(), Is.SameAs(theModule));
        }

        [Test]
        public void WhenUsingBuildToCreateEmptyPipeline_PipelineShouldBeCreated()
        {
            //arrange
            var sut = CreateSut(It.IsAny<PipelineDefinition<TestPipelineStepContext>>());

            // act

            var result = sut.Make(It.IsAny<Configuration>());

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void WhenUsingBuilder_ShouldCreateExecutionContextInsidePipeline()
        {
            //arrange
            var stepList = new List<IPipelineStepDefinintion<TestPipelineStepContext>>
            {
                It.IsAny<PipelineStepDefinintion<TestPipelineStepContext>>()
            };

            var testDefinition = new PipelineDefinition<TestPipelineStepContext>(stepList, TransactionScopeOption.Suppress, TestUtils.PipelineNameForTest);

            var sut = CreateSut(testDefinition);

            var stepExecutingHandler = sut.PipelineStageExecutingHandler;
            var stepExecutedHandler = sut.PipelineStageExecutedHandler;

            // act

            var result = sut.Make(It.IsAny<Configuration>());

            // assert

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<Pipeline<TestPipelineStepContext>>());
            var actualPipeline = (Pipeline<TestPipelineStepContext>) result;

            var execContext = actualPipeline.ExecutionContext;
            Assert.That(execContext, Is.Not.Null);
            Assert.That(execContext.PipelineName, Is.EqualTo(testDefinition.PipelineName));

            Assert.That(execContext.PipelineScope, Is.EqualTo(testDefinition.PipelineScopeOption));
            Assert.That(execContext.StepContext, Is.Null);
            Assert.That(execContext.Steps, Is.Not.Null);
            Assert.That(execContext.Steps.Count, Is.EqualTo(stepList.Count));

            Assert.That(execContext.PipelineStageExecuted, new TestUtils.EventHandlerConstraint<PipelineEventFiredEventArgs>(stepExecutedHandler));
            Assert.That(execContext.PipelineStageExecuting, new TestUtils.EventHandlerConstraint<PipelineEventFiringEventArgs>(stepExecutingHandler));
        }

        [Test]
        public void WhenUsingBuilder_ShouldPassModuleHandlersToDiscoveryFactory()
        {
            //arrange
            var sut = CreateSut(It.IsAny<PipelineDefinition<TestPipelineStepContext>>());

            var moduleInitilizingHandler = sut.ModuleInitializingHandler;
            var moduleInitilizedHandler = sut.ModuleInitializedHandler;

            // act
            sut.Make(It.IsAny<Configuration>());

            // assert
            _mockDiscoveryFactory.Verify(m =>
                m.GetDiscovery<TestPipeline, TestPipelineStepContext>(
                    It.IsAny<TestPipeline>(),
                    It.Is<EventHandler<PipelineModuleInitializedEventArgs>>(ed => ed == moduleInitilizedHandler),
                    It.Is<EventHandler<PipelineModuleInitializingEventArgs>>(ing => ing == moduleInitilizingHandler)));
        }

        [Test]
        public void WhenUsingBuilder_ShouldPassCorrectValuesToDiscoveryInstance()
        {
            // arrange
            var theConfig = It.IsAny<Configuration>();
            var sut = CreateSut(It.IsAny<PipelineDefinition<TestPipelineStepContext>>());
            sut.RegisterModule<ParamlessModule>();
            var modules = sut.Modules;

            // act
            sut.Make(theConfig);

            // assert

            _mockDiscovery.Verify(m =>
                m.ResolvePipeline(
                    It.Is<IEnumerable<PipelineModule<TestPipeline, TestPipelineStepContext>>>(i => i.Count() == modules.Count),
                    It.Is<Configuration>(c => c == theConfig)));
        }

        [Test]
        public void WhenUsingStaticCtorMethod_ShouldReturnBuilder()
        {
            // arrange & act 
            var builder = PipelineBuilder.CreatePipeline<TestPipeline, TestPipelineStepContext>();

            //assert
            Assert.That(builder, Is.Not.Null);
        }
    }
}