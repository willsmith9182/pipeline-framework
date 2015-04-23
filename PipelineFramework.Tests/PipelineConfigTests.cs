using System;
using System.Collections.Specialized;
using System.Linq;
using NUnit.Framework;
using PipelinePlusPlus.Core;
using PipelinePlusPlus.Core.DynamicConfig;
using PipelinePlusPlus.Core.Exceptions;

namespace PipelineFramework.Tests
{
    [TestFixture]
    public class PipelineConfigTests
    {
        [Test]
        public void WhenConfigSectionDoesNotExist_ShouldNotThrowException()
        {
            // Arrange
            var config = TestUtils.GenerateConfigFunc("ConfigWithNoConfigSectionForPipeliner");

            PipelineDynamicModuleConfig<TestContext> sut = null;
            // Act - things happen on construction
            Assert.DoesNotThrow(() =>
            {
                sut = new PipelineDynamicModuleConfig<TestContext>(TestUtils.PipelineNameForTest, config);
            });

            // Assert
            Assert.That(sut, Is.Not.Null);
            Assert.That(sut.Modules.Count, Is.EqualTo(0));
        }

        [Test]
        public void WhenPipelineDoesNotExistInConfig_ShouldThrowException()
        {
            // Arrange

            var config = TestUtils.GenerateConfigFunc("ConfigWithNoPipelinerElement");

            // Act - things happen on construction
            var ex = Assert.Throws<PipelineConfigException>(() =>
            {
                var sut = new PipelineDynamicModuleConfig<TestContext>(TestUtils.PipelineNameForTest, config);
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
                var sut = new PipelineDynamicModuleConfig<TestContext>(TestUtils.PipelineNameForTest, config);
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
            var sut = new PipelineDynamicModuleConfig<TestContext>(TestUtils.PipelineNameForTest, config);

            // Assert
            Assert.That(sut.Modules.Count, Is.EqualTo(1));
        }

        [Test]
        public void WhenConfigHasManyModulesDefined_ShouldLoadModuleDefninitionsFromConfig()
        {
            // Arrange 
            var config = TestUtils.GenerateConfigFunc("ConfigWithFiveModules");

            // Act - things happen on construction
            var sut = new PipelineDynamicModuleConfig<TestContext>(TestUtils.PipelineNameForTest, config);

            // Assert
            Assert.That(sut.Modules.Count, Is.EqualTo(5));
        }

        [Test]
        public void WhenConfigWithKnownModule_ShouldHaveKnownModuleLoaded()
        {
            // Arrange
            var config = TestUtils.GenerateConfigFunc("ConfigWithKnownModule");

            var expected = new ModuleConfig("KnownModule", "KnownNamespace.KnownModule, KnownAssembly", new NameValueCollection());

            // Act - things happen on construction
            var sut = new PipelineDynamicModuleConfig<TestContext>(TestUtils.PipelineNameForTest, config);

            // Assert
            Assert.That(sut.Modules.Count, Is.EqualTo(1));
            var module = sut.Modules.First();

            Assert.That(module.Name, Is.EqualTo(expected.Name));
            Assert.That(module.Type, Is.EqualTo(expected.Type));
            Assert.That(module.Parameters, Is.EqualTo(expected.Parameters));


        }

        [Test]
        public void WhenUnexpectedException_ShouldWrapInConfigException()
        {
            var thrownException = new NotImplementedException("Simulated Exception");

            // Arrange - broken config retrieve - unexpected exception simulation. 
            Func<System.Configuration.Configuration> config = () =>
            {
                throw thrownException;
            };

            // Act - things happen on construction
            var ex = Assert.Throws<PipelineConfigException>(() =>
            {
                var sut = new PipelineDynamicModuleConfig<TestContext>(TestUtils.PipelineNameForTest, config);
            });
            // Assert
            Assert.That(ex.Message, Is.EqualTo("Unexpected error encountered, unable to process config"));
            Assert.That(ex.InnerException.Message, Is.EqualTo(thrownException.Message));
            Assert.That(ex.InnerException, Is.TypeOf<NotImplementedException>());
        }

        [Test]
        public void WhenConfigWithNoModules_ShouldHaveNoModulesInConfig()
        {
            // Arrange
            var config = TestUtils.GenerateConfigFunc("ConfigWithNoModules");

            // Act - things happen on construction
            var sut = new PipelineDynamicModuleConfig<TestContext>(TestUtils.PipelineNameForTest, config);

            // Assert
            Assert.That(sut.Modules.Count, Is.EqualTo(0));
        }

        [Test]
        public void WhenConfigWithKnownModuleWithExtraAttributes_ShouldHaveKnownModuleLoaded()
        {
            // Arrange
            var config = TestUtils.GenerateConfigFunc("ConfigWithExtraParamsOnKnownModule");

            var extraProperties = new NameValueCollection { { "testValue1", "this is a test" }, { "testValue2", "7" } };
            var expected = new ModuleConfig("KnownModule", "KnownNamespace.KnownModule, KnownAssembly", extraProperties);

            // Act - things happen on construction
            var sut = new PipelineDynamicModuleConfig<TestContext>(TestUtils.PipelineNameForTest, config);

            // Assert
            Assert.That(sut.Modules.Count, Is.EqualTo(1));
            var module = sut.Modules.First();

            Assert.That(module.Name, Is.EqualTo(expected.Name));
            Assert.That(module.Type, Is.EqualTo(expected.Type));
            Assert.That(module.Parameters, Is.EqualTo(expected.Parameters));
            // custom test module that compares the keys and values of the NVC
            Assert.That(module.Parameters, new TestUtils.NameValueCollectionConstraint(expected.Parameters));


        }
    }
}
