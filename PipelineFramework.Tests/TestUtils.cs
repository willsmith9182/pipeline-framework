using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using Moq;
using NUnit.Framework.Constraints;
using PipelineFramework.Tests.TestData.Discovery;
using PipelinePlusPlus.Core.DynamicConfig;
using PipelinePlusPlus.Core.EventArgs;
using PipelinePlusPlus.Core.Modules;
using PipelinePlusPlus.Core.Modules.Mananger;
using PipelinePlusPlus.Core.Steps;

namespace PipelineFramework.Tests
{
    public static class TestUtils
    {
        public const string PipelineNameForTest = "TestPipeline";

        /// <summary>
        ///     Helper method, this takes a config name (minus the .config) and creates the delegate used to get
        ///     said config file from the output directory.
        ///     All config files MUST be set to Content and copy to output,
        ///     otherwise the config file that is returned frmm this is a default config file
        ///     which might give you false positives in your tests.
        /// </summary>
        /// <param name="configName">The name of your .config file without the extension</param>
        /// <returns>Delegate to retrieve the Configuration</returns>
        public static Configuration GenerateConfig(string configName)
        {
            var configMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = string.Format("TestData\\Config\\{0}.config", configName)
            };

            return ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
        }

        internal static void VerifyGetConfigCall(this Mock<IDynamicModuleConfig> mock, Configuration config)
        {
            mock.VerifyGetConfigCall(PipelineNameForTest, config);
        }

        internal static void VerifyGetConfigCall(this Mock<IDynamicModuleConfig> mock, string pipelineName,
            Configuration config)
        {
            mock.Verify(m => m.GetConfig(It.Is<String>(s => s == pipelineName), It.Is<Configuration>(c => c == config)));
        }

        internal static void VerifyGetConfigNotCalled(this Mock<IDynamicModuleConfig> mock)
        {
            mock.Verify(m => m.GetConfig(It.IsAny<String>(), It.IsAny<Configuration>()), Times.Never);
        }

        internal static void VerifyRegisterModulesNotCalled<TPipeline>(this Mock<IPipelineModuleManager> mock,
            TPipeline steps) where TPipeline : PipelineSteps
        {
            mock.Verify(
                m =>
                    m.RegisterModules(It.IsAny<TPipeline>(),
                        It.IsAny<IEnumerable<PipelineModule<TPipeline, DiscoveryTestStepContext>>>(),
                        It.IsAny<EventHandler<PipelineModuleInitializingEventArgs>>(),
                        It.IsAny<EventHandler<PipelineModuleInitializedEventArgs>>()), Times.Never);
        }

        internal static void VerifyRegisterDynamicModulesNotCalled<TPipeline>(this Mock<IPipelineModuleManager> mock,
            TPipeline steps) where TPipeline : PipelineSteps
        {
            mock.Verify(
                m =>
                    m.RegisterDynamicModules<TPipeline, DiscoveryTestStepContext>(It.IsAny<TPipeline>(),
                        It.IsAny<PipelineDynamicModuleConfig>(),
                        It.IsAny<EventHandler<PipelineModuleInitializingEventArgs>>(),
                        It.IsAny<EventHandler<PipelineModuleInitializedEventArgs>>()), Times.Never);
        }

        internal static void VerifyRegisterModulesCall<TPipeline>(this Mock<IPipelineModuleManager> mock,
            Expression<Func<IEnumerable<PipelineModule<TPipeline, DiscoveryTestStepContext>>, bool>> modules,
            EventHandler<PipelineModuleInitializingEventArgs> initialiseEvents,
            EventHandler<PipelineModuleInitializedEventArgs> initialisedEvents, TPipeline steps)
            where TPipeline : PipelineSteps
        {
            mock.Verify(
                m =>
                    m.RegisterModules(It.IsAny<TPipeline>(), It.Is(modules),
                        It.Is<EventHandler<PipelineModuleInitializingEventArgs>>(i => i == initialiseEvents),
                        It.Is<EventHandler<PipelineModuleInitializedEventArgs>>(i => i == initialisedEvents)));
        }

        internal static void VerifyRegisterDynamicModulesCall<TPipeline>(this Mock<IPipelineModuleManager> mock,
            Mock<PipelineDynamicModuleConfig> config, EventHandler<PipelineModuleInitializingEventArgs> initialiseEvents,
            EventHandler<PipelineModuleInitializedEventArgs> initialisedEvents, TPipeline steps)
            where TPipeline : PipelineSteps
        {
            mock.Verify(
                m =>
                    m.RegisterDynamicModules<TPipeline, DiscoveryTestStepContext>(It.IsAny<TPipeline>(),
                        It.Is<PipelineDynamicModuleConfig>(i => i == config.Object),
                        It.Is<EventHandler<PipelineModuleInitializingEventArgs>>(i => i == initialiseEvents),
                        It.Is<EventHandler<PipelineModuleInitializedEventArgs>>(i => i == initialisedEvents)));
        }

        internal static IEnumerable<PipelineModule<TPipeline, DiscoveryTestStepContext>> CreateEmptyModuleCollection
            <TPipeline>(this TPipeline steps) where TPipeline : PipelineSteps
        {
            return new List<PipelineModule<TPipeline, DiscoveryTestStepContext>>();
        }

        internal class EventHandlerConstraint<T> : Constraint
        {
            private readonly EventHandler<T> _expected;

            public EventHandlerConstraint(EventHandler<T> expected)
            {
                _expected = expected;
            }

            public override bool Matches(object actualValue)
            {
                actual = actualValue;
                var del = actualValue as EventHandler<T>;
                return Match(del, _expected);
            }

            // ncrunch: no coverage start
            public override void WriteDescriptionTo(MessageWriter writer)
            {
                writer.WriteExpectedValue(_expected);
            }

            // ncrunch: no coverage end
            private static bool Match(EventHandler<T> actual, EventHandler<T> expected)
            {
                // ncrunch: no coverage start
                if (actual == null || expected == null)
                {
                    return false;
                }
                // ncrunch: no coverage end
                return actual.Equals(expected);
            }
        }

        //custom constraint for Nunit asertations. 
        internal class NameValueCollectionConstraint : Constraint
        {
            private readonly NameValueCollection _expected;

            public NameValueCollectionConstraint(NameValueCollection expected)
            {
                _expected = expected;
            }

            public override bool Matches(object actualValue)
            {
                // why this isn't done by the base i don't know. Bit silly tbh...
                actual = actualValue;

                // safe cast
                var collection = actualValue as NameValueCollection;

                // if it's a nvc and it matches, otherwise nope. 
                return collection != null && Match(collection, _expected);
            }

            public override void WriteDescriptionTo(MessageWriter writer)
            {
                writer.WriteExpectedValue(_expected);
            }

            private static bool Match(NameValueCollection actual, NameValueCollection expected)
            {
                var expectedKeys = expected.AllKeys.ToList();
                var actualKeys = actual.AllKeys.ToList();

                // not the right ammount of keys to start with
                if (expectedKeys.Count != actualKeys.Count)
                {
                    return false;
                }

                // intersect the keys, find all the ones that match
                var matchingKeys = expectedKeys.Intersect(actualKeys)
                    .ToList();

                // if not all of them match (keys and values)
                return matchingKeys.Count() == expectedKeys.Count() &&
                       expectedKeys.All(key => actual[key] == expected[key]);
            }
        }
    }
}