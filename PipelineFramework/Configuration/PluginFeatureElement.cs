using System.Configuration;
using System.Linq;

namespace Pipeline.Configuration
{
    // not an implemented feature of this framework yet

    [ConfigurationCollection(typeof (PluginElement))]
    public class PluginFeatureElement : ConfigurationElementCollection
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string) base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("defaultPlugin", DefaultValue = "")]
        public string DefaultPlugin
        {
            get { return (string) base["defaultPlugin"]; }
            set { base["defaultPlugin"] = value; }
        }

        internal PluginElement this[int index]
        {
            get { return (PluginElement) BaseGet(index); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new PluginElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PluginElement) element).Name;
        }

        internal PluginElement GetByName(string name)
        {
            return this.Cast<PluginElement>().FirstOrDefault(item => item.Name == name);
        }
    }
}