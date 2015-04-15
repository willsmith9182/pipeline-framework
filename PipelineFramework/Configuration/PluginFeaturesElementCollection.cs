using System.Configuration;
using System.Linq;

namespace Pipeline.Configuration
{
    // not an implemented feature of this framework yet

    [ConfigurationCollection(typeof (PluginFeatureElement), AddItemName = "plugin")]
    public class PluginFeaturesElementCollection : ConfigurationElementCollection
    {
        internal PluginFeatureElement this[int index]
        {
            get { return (PluginFeatureElement) BaseGet(index); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new PluginFeatureElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PluginFeatureElement) element).Name;
        }

        internal PluginFeatureElement GetByName(string pluginFeatureName)
        {
            return this.Cast<PluginFeatureElement>().FirstOrDefault(item => item.Name == pluginFeatureName);
        }
    }
}