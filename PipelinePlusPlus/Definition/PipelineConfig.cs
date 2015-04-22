using System;
using System.Configuration;
using PipelinePlusPlus.Core;

namespace PipelinePlusPlus.Definition
{
    public class PipelineConfig<TContext>
    {
        public PipelineConfig(string name, Func<System.Configuration.Configuration> getConfig)
        {
            Name = name;
            Modules = new ModulesConfig();
            try
            {
                var config = getConfig();

                var section = config.GetPipelineFrameworkConfiguration();

                //if there is no config section for this pipeline, not to worry. 
                //set up element and don't load any modules from configs. 
                if (section == null) return;

                var pipelineElement = section.Pipelines.GetByName(name);

                //if there is no config section for this pipeline, not to worry. 
                //set up element and don't load any modules from configs. 
                if (pipelineElement == null)
                    throw new PipelineConfigException(
                        string.Format("Unable to load config for pipeline '{0}', please check your app/web.config", name));

                foreach (ProviderSettings item in pipelineElement.Modules)
                    Modules.Add(item.Name, item.Type, item.Parameters);
            }
            catch (PipelineConfigException e)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new PipelineConfigException("Unexpected error encountered, unable to process config", e);
            }
        }

        public string Name { get; private set; }
        public ModulesConfig Modules { get; private set; }
    }
}