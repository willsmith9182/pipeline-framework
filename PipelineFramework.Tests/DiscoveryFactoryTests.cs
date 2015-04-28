using System;
using Moq;
using NUnit.Framework;
using PipelineFramework.Tests.TestData.Builder;
using PipelinePlusPlus.Builder;
using PipelinePlusPlus.Core.Discovery;
using PipelinePlusPlus.Core.DynamicConfig;
using PipelinePlusPlus.Core.EventArgs;
using PipelinePlusPlus.Core.Modules.Mananger;

namespace PipelineFramework.Tests
{
    [TestFixture]
    public class DiscoveryFactoryTests
    {
        private Mock<IDynamicModuleConfig> _mockDynamicConfig;
        private Mock<IPipelineModuleManager> _mockModuleManager;


        private void Setup()
        {
            _mockDynamicConfig = new Mock<IDynamicModuleConfig>();
            _mockModuleManager = new Mock<IPipelineModuleManager>();
        }

        private IDiscoveryFactory CreateSut(Action postSetup = null)
        {
            // ncrunch: no coverage start
            Setup();
            if (postSetup != null)
                postSetup();

            return new DiscoveryFactory(_mockDynamicConfig.Object, _mockModuleManager.Object);
            // ncrunch: no coverage end
        }

        [Test]
        public void WhenUsingDiscoveryFactory_ShouldReturnDiscovery()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.GetDiscovery<TestPipeline, TestPipelineStepContext>(
                It.IsAny<TestPipeline>(),
                It.IsAny<EventHandler<PipelineModuleInitializedEventArgs>>(),
                It.IsAny<EventHandler<PipelineModuleInitializingEventArgs>>());

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<PipelineDiscovery<TestPipeline, TestPipelineStepContext>>());

        }
    }
}
