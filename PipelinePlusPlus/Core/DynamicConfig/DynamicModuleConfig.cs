using System;
using System.Configuration;
using PipelinePlusPlus.Core.ConfigurationElements;
using PipelinePlusPlus.Core.Exceptions;

namespace PipelinePlusPlus.Core.DynamicConfig
{
    internal class DynamicModuleConfig : IDynamicModuleConfig
    {
        public PipelineDynamicModuleConfig GetConfig(string name, Configuration appConfig)
        {
            var config = new PipelineDynamicModuleConfig(name);

            try
            {
                var section = appConfig.GetPipelineFrameworkConfiguration();

                //if there is no config section for this pipeline, not to worry. 
                //set up element and don't load any modules from configs. 
                if (section != null)
                {
                    var pipelineElement = section.Pipelines.GetByName(name);

                    if (pipelineElement == null)
                    {
                        throw new PipelineConfigException(string.Format("Unable to load config for pipeline '{0}', please check your app/web.config", name));
                    }

                    var dynamicModules = new ModulesConfig();

                    foreach (ProviderSettings item in pipelineElement.Modules)
                    {
                        dynamicModules.Add(item.Name, item.Type, item.Parameters);
                    }
                    config.Modules = dynamicModules;
                }
            }
            catch (PipelineConfigException e)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new PipelineConfigException("Unexpected error encountered, unable to process config", e);
            }
            return config;
        }
    }
}