using System.Configuration;

namespace PipelinePlusPlus.Core.DynamicConfig
{
    internal interface IDynamicModuleConfig
    {
        PipelineDynamicModuleConfig GetConfig(string name, Configuration appConfig);
    }
}