namespace PipelinePlusPlus.Core.ConfigurationElements
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