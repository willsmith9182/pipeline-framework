using NUnit.Framework;
using PipelinePlusPlus.Core;
using PipelinePlusPlus.Definition;

namespace PipelineFramework.Tests
{
    [TestFixture]
    public class PipelineConfigTests
    {
        [Test]
        public void WhenConfigSectionDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var config = TestUtils.GenerateConfigFunc("ConfigWithNoConfigSectionForPipeliner");


            // Act - things happen on construction
            var ex = Assert.Throws<PipelineConfigException>(() =>
            {
                var sut = new PipelineConfig(TestUtils.PipelineNameForTest, config);
            });

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Unable to load pipeliner config section, please check your app/web.config"));
        }

        [Test]
        public void WhenPipelineDoesNotExistInConfig_ShouldThrowException()
        {
            // Arrange

            var config = TestUtils.GenerateConfigFunc("ConfigWithNoPipelinerElement");

            // Act - things happen on construction
            var ex = Assert.Throws<PipelineConfigException>(() =>
            {
                var sut = new PipelineConfig(TestUtils.PipelineNameForTest, config);
            });

            // Assert
            Assert.That(ex.Message, Is.EqualTo(string.Format("Unable to load config for pipeline '{0}', please check your app/web.config", TestUtils.PipelineNameForTest)));
        }
        

        [Test]
        public void WhenConfigHasInvalidXml_ShouldNotLoadAnyConfig()
        {
            // Arrange
            var config = TestUtils.GenerateConfigFunc("ConfigWithInvalidXml");

            // Act - things happen on construction
            var ex = Assert.Throws<PipelineConfigException>(() =>
            {
                var sut = new PipelineConfig(TestUtils.PipelineNameForTest, config);
            });

            // Assert
            Assert.That(ex.Message, Is.EqualTo(string.Format("Unable to load config for pipeline '{0}', please check your app/web.config", TestUtils.PipelineNameForTest)));
        }

        [Test]
        public void WhenConfigHasModulesDefined_ShouldLoadModuleDefninitionsFromConfig()
        {
            // Arrange
            var config = TestUtils.GenerateConfigFunc("ConfigWithOneModule");

            // Act - things happen on construction
            var sut = new PipelineConfig(TestUtils.PipelineNameForTest, config);

            // Assert
            Assert.That(sut.Modules.Count, Is.EqualTo(1));
        }

        [Test]
        public void WhenConfigHasManyModulesDefined_ShouldLoadModuleDefninitionsFromConfig()
        {
            // Arrange 
            var config = TestUtils.GenerateConfigFunc("ConfigWithFiveModules");

            // Act - things happen on construction
            var sut = new PipelineConfig(TestUtils.PipelineNameForTest, config);

            // Assert
            Assert.That(sut.Modules.Count, Is.EqualTo(5));
        }
    }
}
