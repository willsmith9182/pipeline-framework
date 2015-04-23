using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace PipelineFramework.Tests
{
    public static class TestUtils
    {
        public const string PipelineNameForTest = "TestPipeline";

        /// <summary>
        /// Helper method, this takes a config name (minus the .config) and creates the delegate used to get 
        /// said config file from the output directory. 
        /// All config files MUST be set to Content and copy to output, 
        /// otherwise the config file that is returned frmm this is a default config file 
        /// which might give you false positives in your tests.  
        /// </summary>
        /// <param name="configName">The name of your .config file without the extension</param>
        /// <returns>Delegate to retrieve the Configuration</returns>
        public static Func<Configuration> GenerateConfigFunc(string configName)
        {
            var configMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = string.Format("TestData\\Config\\{0}.config", configName)
            };

            return () => ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
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
                if (expectedKeys.Count != actualKeys.Count) return false;

                // intersect the keys, find all the ones that match
                var matchingKeys = expectedKeys.Intersect(actualKeys).ToList();

                // if not all of them match (keys and values)
                return matchingKeys.Count() == expectedKeys.Count() && expectedKeys.All(key => actual[key] == expected[key]);
            }
        }

        internal static NameValueCollectionConstraint NameValueCollectionEqualTo(this Is issm, NameValueCollection expected)
        {
            return new NameValueCollectionConstraint(expected);
        }
    }
}
