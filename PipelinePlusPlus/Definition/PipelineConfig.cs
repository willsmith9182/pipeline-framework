using System.Configuration;
using PipelinePlusPlus.Configuration;

namespace PipelinePlusPlus.Definition
{
    public class PipelineConfig
    {
        public PipelineConfig(string name)
        {
            Modules = new ModulesConfig();
            Name = name;

            var section = (PipelineFrameworkConfigurationSection) (ConfigurationManager.GetSection("pipelineFramework"));

            // if there is no config section for this pipeline, not to worry. 
            // set up element and don't load any modules from configs. 
            if (section == null) return;

            var pipelineElement = section.Pipelines.GetByName(name);

            // if there is no config section for this pipeline, not to worry. 
            // set up element and don't load any modules from configs. 
            if (pipelineElement == null) return;


            foreach (ProviderSettings item in pipelineElement.Modules)
                Modules.Add(item.Name, item.Type, item.Parameters);
        }

        public string Name { get; private set; }
        public ModulesConfig Modules { get; private set; }
    }
}