using System.Configuration;

namespace PipelinePlusPlus.Configuration
{
    public class PipelineElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string) base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("modules", IsRequired = true)]
        public ProviderSettingsCollection Modules
        {
            get { return (ProviderSettingsCollection) base["modules"]; }
            set { base["modules"] = value; }
        }
    }
}