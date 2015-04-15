using System.Configuration;
using System.Linq;

namespace Pipeline.Configuration
{
    [ConfigurationCollection(typeof (PipelineElement), AddItemName = "provider")]
    public class ProviderFeaturesElementCollection : ConfigurationElementCollection
    {
        internal ProviderFeatureElement this[int index]
        {
            get { return (ProviderFeatureElement) BaseGet(index); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ProviderFeatureElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ProviderFeatureElement) element).Name;
        }

        internal ProviderFeatureElement GetByName(string providerFeatureName)
        {
            return this.Cast<ProviderFeatureElement>().FirstOrDefault(item => item.Name == providerFeatureName);
        }
    }
}