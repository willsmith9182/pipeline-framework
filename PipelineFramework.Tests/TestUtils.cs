using System;
using System.Configuration;

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
        public static Func<System.Configuration.Configuration> GenerateConfigFunc(string configName)
        {
            var configMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = string.Format("TestData\\Config\\{0}.config", configName)
            };

            return () => ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
        }

        
    }
}
