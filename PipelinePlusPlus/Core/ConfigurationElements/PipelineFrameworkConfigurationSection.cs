using System.Configuration;

namespace PipelinePlusPlus.Core.ConfigurationElements
{
    public class PipelineFrameworkConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("pipelines")]
        public PipelineElementCollection Pipelines { get { return (PipelineElementCollection) base["pipelines"]; } }
    }
}