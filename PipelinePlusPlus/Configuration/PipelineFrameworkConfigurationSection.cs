using System.Configuration;

namespace PipelinePlusPlus.Configuration
{
    public class PipelineFrameworkConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("pipelines")]
        public PipelineElementCollection Pipelines
        {
            get { return (PipelineElementCollection)base["pipelines"]; }
        }
    }
}