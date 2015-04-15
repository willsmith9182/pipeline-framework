using System.Configuration;

namespace Pipeline.Configuration
{
    public class ProviderElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string) base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get { return (string) base["type"]; }
            set { base["type"] = value; }
        }
    }
}