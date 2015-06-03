using System;
using System.Collections.Specialized;
using System.Linq;
using NUnit.Framework;
using PipelinePlusPlus.Core.DynamicConfig;
using PipelinePlusPlus.Core.Exceptions;

namespace PipelineFramework.Tests
{
    [TestFixture]
    public class DynamicModuleConfigTests
    {
        private DynamicModuleConfig _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new DynamicModuleConfig();
        }

        [Test]
        public void WhenConfigSectionDoesNotExist_ShouldNotThrowException()
        {
            // Arrange
            var config = TestUtils.GenerateConfig("ConfigWithNoConfigSectionForPipeliner");

            PipelineDynamicModuleConfig result = null;
            // Act
            Assert.DoesNotThrow(() => { result = _sut.GetConfig(TestUtils.PipelineNameForTest, config); });

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Modules.Count, Is.EqualTo(0));
        }

        [Test]
        public void WhenPipelineDoesNotExistInConfig_ShouldThrowException()
        {
            // Arrange
            var config = TestUtils.GenerateConfig("ConfigWithNoPipelinerElement");


            // Act
            var ex = Assert.Throws<PipelineConfigException>(() =>
            {
                var result = _sut.GetConfig(TestUtils.PipelineNameForTest, config);
            });

            // Assert
            Assert.That(ex.Message, Is.EqualTo(string.Format("Unable to load config for pipeline '{0}', please check your app/web.config", TestUtils.PipelineNameForTest)));
        }

        [Test]
        public void WhenConfigHasInvalidXml_ShouldNotLoadAnyConfig()
        {
            // Arrange
            var config = TestUtils.GenerateConfig("ConfigWithInvalidXml");


            // Act
            var ex = Assert.Throws<PipelineConfigException>(() => { var result = _sut.GetConfig(TestUtils.PipelineNameForTest, config); });

            // Assert
            Assert.That(ex.Message, Is.EqualTo(string.Format("Unable to load config for pipeline '{0}', please check your app/web.config", TestUtils.PipelineNameForTest)));
        }

        [Test]
        public void WhenConfigHasModulesDefined_ShouldLoadModuleDefninitionsFromConfig()
        {
            // Arrange
            var config = TestUtils.GenerateConfig("ConfigWithOneModule");


            // Act
            var result = _sut.GetConfig(TestUtils.PipelineNameForTest, config);

            // Assert
            Assert.That(result.Modules.Count, Is.EqualTo(1));
        }

        [Test]
        public void WhenConfigHasManyModulesDefined_ShouldLoadModuleDefninitionsFromConfig()
        {
            // Arrange 
            var config = TestUtils.GenerateConfig("ConfigWithFiveModules");

            // Act
            var result = _sut.GetConfig(TestUtils.PipelineNameForTest, config);

            // Assert
            Assert.That(result.Modules.Count, Is.EqualTo(5));
        }

        [Test]
        public void WhenConfigWithKnownModule_ShouldHaveKnownModuleLoaded()
        {
            // Arrange
            var config = TestUtils.GenerateConfig("ConfigWithKnownModule");

            var expected = new ModuleConfig("KnownModule", "KnownNamespace.KnownModule, KnownAssembly", new NameValueCollection());

            // Act
            var result = _sut.GetConfig(TestUtils.PipelineNameForTest, config);

            // Assert
            Assert.That(result.Modules.Count, Is.EqualTo(1));
            var module = result.Modules.First();

            Assert.That(module.Name, Is.EqualTo(expected.Name));
            Assert.That(module.Type, Is.EqualTo(expected.Type));
            Assert.That(module.Parameters, Is.EqualTo(expected.Parameters));
        }

        [Test]
        public void WhenUnexpectedException_ShouldWrapInConfigException()
        {
            // Act
            var ex = Assert.Throws<PipelineConfigException>(() => { var result = _sut.GetConfig(TestUtils.PipelineNameForTest, null); });
            // Assert
            Assert.That(ex.Message, Is.EqualTo("Unexpected error encountered, unable to process config"));
            Assert.That(ex.InnerException.Message, Is.EqualTo("Object reference not set to an instance of an object."));
            Assert.That(ex.InnerException, Is.TypeOf<NullReferenceException>());
        }

        [Test]
        public void WhenConfigWithNoModules_ShouldHaveNoModulesInConfig()
        {
            // Arrange
            var config = TestUtils.GenerateConfig("ConfigWithNoModules");

            // Act
            var result = _sut.GetConfig(TestUtils.PipelineNameForTest, config);

            // Assert
            Assert.That(result.Modules.Count, Is.EqualTo(0));
        }

        [Test]
        public void WhenConfigWithKnownModuleWithExtraAttributes_ShouldHaveKnownModuleLoaded()
        {
            // Arrange
            var config = TestUtils.GenerateConfig("ConfigWithExtraParamsOnKnownModule");

            var extraProperties = new NameValueCollection
            {
                {
                    "testValue1", "this is a test"
                },
                {
                    "testValue2", "7"
                }
            };
            var expected = new ModuleConfig("KnownModule", "KnownNamespace.KnownModule, KnownAssembly", extraProperties);

            // Act
            var result = _sut.GetConfig(TestUtils.PipelineNameForTest, config);

            // Assert
            Assert.That(result.Modules.Count, Is.EqualTo(1));
            var module = result.Modules.First();

            Assert.That(module.Name, Is.EqualTo(expected.Name));
            Assert.That(module.Type, Is.EqualTo(expected.Type));
            Assert.That(module.Parameters, Is.EqualTo(expected.Parameters));
            // custom test module that compares the keys and values of the NVC
            Assert.That(module.Parameters, new TestUtils.NameValueCollectionConstraint(expected.Parameters));
        }
    }
}