using System.Configuration;
using System.Linq;

namespace PipelinePlusPlus.Core.ConfigurationElements
{
    // ncrunch: no coverage start
    [ConfigurationCollection(typeof (PipelineElement), AddItemName = "pipeline")]
    public class PipelineElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new PipelineElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PipelineElement) element).Name;
        }

        internal PipelineElement GetByName(string name)
        {
            return this.Cast<PipelineElement>()
                .FirstOrDefault(item => item.Name == name);
        }
    }
}