using System;
using System.Configuration;
using PipelinePlusPlus.Configuration;
using PipelinePlusPlus.Core;

namespace PipelinePlusPlus.Definition
{
    public static class ConfigExtensions
    {
        public static PipelineFrameworkConfigurationSection GetPipelineFrameworkConfiguration(this System.Configuration.Configuration config)
        {
            return config.GetSection("pipeliner") as PipelineFrameworkConfigurationSection;
        }
    }

    public class PipelineConfig<TContext>
    {
        public PipelineConfig(string name, Func<System.Configuration.Configuration> getConfig)
        {
            Name = name;
            Modules = new ModulesConfig();

            var config = getConfig();

            var section = config.GetPipelineFrameworkConfiguration();

            //if there is no config section for this pipeline, not to worry. 
            //set up element and don't load any modules from configs. 
            if (section == null) return;

            var pipelineElement = section.Pipelines.GetByName(name);

            //if there is no config section for this pipeline, not to worry. 
            //set up element and don't load any modules from configs. 
            if (pipelineElement == null) throw new PipelineConfigException(string.Format("Unable to load config for pipeline '{0}', please check your app/web.config", name));



            foreach (ProviderSettings item in pipelineElement.Modules)
                Modules.Add(item.Name, item.Type, item.Parameters);
        }

        public string Name { get; private set; }
        public ModulesConfig Modules { get; private set; }
    }
}