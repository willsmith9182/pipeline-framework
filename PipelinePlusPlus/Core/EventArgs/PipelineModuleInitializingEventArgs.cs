using System.Collections.Generic;
using System.Linq;
using PipelinePlusPlus.Core.DynamicConfig;

namespace PipelinePlusPlus.Core.EventArgs
{
    public class PipelineModuleInitializingEventArgs : PipelineCancelEventArgsBase
    {
        public PipelineModuleInitializingEventArgs(string pipelineName, ModuleConfig module) : base(pipelineName)
        {
            ModuleName = module.Name;
            ModuleType = module.Type;
            AdditionalParameters = new Dictionary<string, string>();

            foreach (var
                key
                in
                module.Parameters.AllKeys.Where(key => !AdditionalParameters.ContainsKey(key)))
            {
                AdditionalParameters.Add(key, module.Parameters[key]);
            }
        }

        public PipelineModuleInitializingEventArgs(string pipelineName, string moduleName, string typeName) : base(pipelineName)
        {
            ModuleName = moduleName;
            ModuleType = typeName;
            AdditionalParameters = new Dictionary<string, string>();
        }

        public string ModuleName { get; private set; }
        public string ModuleType { get; private set; }
        public IDictionary<string, string> AdditionalParameters { get; private set; }
    }
}