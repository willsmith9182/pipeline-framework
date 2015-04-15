using System.Configuration;
using System.Linq;

namespace Pipeline.Configuration
{
    [ConfigurationCollection(typeof (ProviderElement))]
    public class ProviderFeatureElement : ConfigurationElementCollection
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string) base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("defaultProvider", DefaultValue = "")]
        public string DefaultProvider
        {
            get { return (string) base["defaultProvider"]; }
            set { base["defaultProvider"] = value; }
        }

        internal ProviderElement this[int index]
        {
            get { return (ProviderElement) BaseGet(index); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ProviderElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ProviderElement) element).Name;
        }

        internal ProviderElement GetByName(string name)
        {
            return this.Cast<ProviderElement>().FirstOrDefault(item => item.Name == name);
        }
    }
}