using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Transactions;
using Moq;
using NUnit.Framework;
using PipelineFramework.Tests.TestData.Definition;
using PipelinePlusPlus.Builder;
using PipelinePlusPlus.Core;
using PipelinePlusPlus.Core.Discovery;
using PipelinePlusPlus.Core.DynamicConfig;
using PipelinePlusPlus.Core.EventArgs;
using PipelinePlusPlus.Core.Modules;
using PipelinePlusPlus.Core.Modules.Mananger;
using PipelinePlusPlus.Core.Steps;

namespace PipelineFramework.Tests
{
    [TestFixture]
    public class PipelineBuilderTests
    {
        private Mock<IPipelineDiscovery> _mockDiscovery;
        private Mock<IPipelineModuleManager> _mockModuleMananger;
        private Mock<IDynamicModuleConfig> _mockDynamicConfig;
        private Mock<PipelineDynamicModuleConfig> _mockPipelineDynamicModuleConfig;

        [SetUp]
        public void Setup()
        {
            _mockDiscovery = new Mock<IPipelineDiscovery>(MockBehavior.Strict);
            _mockModuleMananger = new Mock<IPipelineModuleManager>(MockBehavior.Strict);
            _mockDynamicConfig = new Mock<IDynamicModuleConfig>(MockBehavior.Strict);
            _mockPipelineDynamicModuleConfig = new Mock<PipelineDynamicModuleConfig>(MockBehavior.Loose, TestUtils.PipelineNameForTest);
        }


        [Test]
        public void WhenUsingBuilderToAddModuleInitializingHandler_PipelineShouldHaveCorrectNumberOfHandlers()
        {
            // arrange
            var testDefinition = new PipelineDefinition<TestPipelineStepContext>(new List<IPipelineStepDefinintion<TestPipelineStepContext>>(), TransactionScopeOption.Suppress, "TestPipeline");

            var emptyconfig = TestUtils.GenerateConfig(TestUtils.PipelineNameForTest);

            // return a predefined result
            _mockDiscovery.Setup(d => d.Discover<TestPipelineStepContext>(It.IsAny<TestPipeline>())).Returns(testDefinition);

            // do nothing.
            _mockModuleMananger.Setup(m => m.RegisterDynamicModules<TestPipeline, TestPipelineStepContext>(It.IsAny<TestPipeline>(), It.IsAny<PipelineDynamicModuleConfig>(), It.IsAny<EventHandler<PipelineModuleInitializingEventArgs>>(), It.IsAny<EventHandler<PipelineModuleInitializedEventArgs>>()));
            _mockModuleMananger.Setup(m => m.RegisterModules(It.IsAny<TestPipeline>(), It.IsAny<IEnumerable<PipelineModule<TestPipeline, TestPipelineStepContext>>>(), It.IsAny<EventHandler<PipelineModuleInitializingEventArgs>>(), It.IsAny<EventHandler<PipelineModuleInitializedEventArgs>>()));

            _mockDynamicConfig.Setup(m => m.GetConfig(It.IsAny<String>(), It.IsAny<Configuration>())).Returns(_mockPipelineDynamicModuleConfig.Object);

            Action<PipelineModuleInitializingEventArgs> testHandler = e =>
                                                                      {
                                                                          // do something
                                                                      };

            var sut = new PipelineBuilder<TestPipeline, TestPipelineStepContext>(_mockModuleMananger.Object, _mockDiscovery.Object, _mockDynamicConfig.Object);

            // act
            sut.OnModuleInitialize(testHandler);
            var result = sut.Make(emptyconfig);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<Pipeline<TestPipelineStepContext>>());
            

            _mockModuleMananger.VerifyRegisterDynamicModulesCall(
                _mockPipelineDynamicModuleConfig,
                i => i != null && i.GetInvocationList().Count() == 1,
                j => j == null);

            _mockModuleMananger.VerifyRegisterModulesCall(
                m => !m.Any(),
                i => i != null && i.GetInvocationList().Count() == 1,
                j => j == null);


        }


    }
}