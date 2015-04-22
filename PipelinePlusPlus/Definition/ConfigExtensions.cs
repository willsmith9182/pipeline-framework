using PipelinePlusPlus.Configuration;

namespace PipelinePlusPlus.Definition
{
    public static class ConfigExtensions
    {
        public static PipelineFrameworkConfigurationSection GetPipelineFrameworkConfiguration(
            this System.Configuration.Configuration config)
        {
            return config.GetSection("pipeliner") as PipelineFrameworkConfigurationSection;
        }
    }
}