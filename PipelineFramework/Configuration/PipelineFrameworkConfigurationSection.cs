using System.Configuration;

namespace Pipeline.Configuration
{
    public class PipelineFrameworkConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("pipelines")]
        public PipelineElementCollection Pipelines
        {
            get { return (PipelineElementCollection) base["pipelines"]; }
        }

        [ConfigurationProperty("providers")]
        public ProviderFeaturesElementCollection Providers
        {
            get { return (ProviderFeaturesElementCollection) base["providers"]; }
        }

        // not an implemented feature of this framework yet
        [ConfigurationProperty("plugins")]
        public PluginFeaturesElementCollection Plugins
        {
            get { return (PluginFeaturesElementCollection) base["plugins"]; }
        }
    }
}