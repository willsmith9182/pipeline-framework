using System.Collections.Generic;
using System.Collections.Specialized;

namespace PipelinePlusPlus.Core.DynamicConfig
{
    public class ModulesConfig : List<ModuleConfig>
    {
        public void Add(string name, string type)
        {
            Add(name, type, new NameValueCollection());
        }

        public void Add(string name, string type, NameValueCollection parameters)
        {
            Add(new ModuleConfig(name, type, parameters));
        }
    }
}