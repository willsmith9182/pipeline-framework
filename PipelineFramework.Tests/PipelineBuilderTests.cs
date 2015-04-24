using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Remoting.Channels;
using System.Transactions;
using Moq;
using NUnit.Framework;
using PipelineFramework.Tests.TestData.Builder;
using PipelinePlusPlus.Builder;
using PipelinePlusPlus.Core;
using PipelinePlusPlus.Core.Discovery;
using PipelinePlusPlus.Core.EventArgs;
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

        public IPipelineBuilder<TestPipeline, TestPipelineStepContext> CreateSut(Action postSetup = null)
        {
            Setup();
            if (postSetup != null)
                postSetup();

            return new PipelineBuilder<TestPipeline, TestPipelineStepContext>(_mockDiscoveryFactory.Object);
        }

        [Test]
        public void WhenUsingBuilderToAddModuleInitializingHandler_PipelineShouldHaveCorrectNumberOfHandlers()
        {
            //arrange
            var testDefinition = new PipelineDefinition<TestPipelineStepContext>(new List<IPipelineStepDefinintion<TestPipelineStepContext>>(), TransactionScopeOption.Suppress, "TestPipeline");

            var emptyconfig = TestUtils.GenerateConfig(TestUtils.PipelineNameForTest);

            var sut = CreateSut(() =>
            {
                _mockDiscovery.Setup(m => m.ResolvePipeline(It.IsAny<IEnumerable<PipelineModule<TestPipeline, TestPipelineStepContext>>>(), It.IsAny<Configuration>())).Returns(testDefinition);
            });


            // act
            
            var result = sut.Make(emptyconfig);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<Pipeline<TestPipelineStepContext>>());


           
        }
    }


}