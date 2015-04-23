using System;
using System.Collections.Generic;
using System.Transactions;
using Moq;
using NUnit.Framework;
using PipelineFramework.Tests.TestData.Definition;
using PipelinePlusPlus.Builder;
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

        [SetUp]
        public void Setup()
        {
            _mockDiscovery = new Mock<IPipelineDiscovery>(MockBehavior.Strict);
            _mockModuleMananger = new Mock<IPipelineModuleManager>(MockBehavior.Strict);
        }


        [Test]
        public void WhenUsingBuilderToAddModuleInitializingHandler_PipelineShouldHaveCorrectNumberOfHandlers()
        {
            // arrange
            var testDefinition = new PipelineDefinition<TestPipelineStepContext>(new List<IPipelineStepDefinintion<TestPipelineStepContext>>(), TransactionScopeOption.Suppress, "TestPipeline");

            // return a predefined result
            _mockDiscovery.Setup(d => d.Discover<TestPipelineStepContext>(It.IsAny<TestPipeline>())).Returns(testDefinition);

            // do nothing.
          //  _mockModuleMananger.Setup(m => m.RegisterDynamicModules(It.IsAny<TestPipeline>(), It.IsAny<PipelineDynamicModuleConfig>(), It.IsAny<EventHandler<PipelineModuleInitializingEventArgs>>(), It.IsAny<EventHandler<PipelineModuleInitializedEventArgs>>()));
            _mockModuleMananger.Setup(m => m.RegisterModules(It.IsAny<TestPipeline>(), It.IsAny<IEnumerable<PipelineModule<TestPipeline, TestPipelineStepContext>>>(), It.IsAny<EventHandler<PipelineModuleInitializingEventArgs>>(), It.IsAny<EventHandler<PipelineModuleInitializedEventArgs>>()));

            
            var sut = new PipelineBuilder<TestPipeline, TestPipelineStepContext>(_mockModuleMananger.Object, _mockDiscovery.Object,null);
            // act


            // assert

        }
    }
}