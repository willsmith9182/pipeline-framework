using System.Configuration;

namespace PipelinePlusPlus.Core.ConfigurationElements
{
    public static class ConfigExtensions
    {
        public static PipelineFrameworkConfigurationSection GetPipelineFrameworkConfiguration(this Configuration config) { return config.GetSection("pipeliner") as PipelineFrameworkConfigurationSection; }
    }
}