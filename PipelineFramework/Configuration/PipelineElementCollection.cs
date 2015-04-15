using System.Configuration;
using System.Linq;

namespace Pipeline.Configuration
{
    [ConfigurationCollection(typeof (PipelineElement), AddItemName = "pipeline")]
    public class PipelineElementCollection : ConfigurationElementCollection
    {
        internal PipelineElement this[int index]
        {
            get { return (PipelineElement) BaseGet(index); }
        }

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
            return this.Cast<PipelineElement>().FirstOrDefault(item => item.Name == name);
        }
    }
}