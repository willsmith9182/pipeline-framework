using System.Collections.Generic;
using System.Collections.Specialized;

namespace PipelinePlusPlus.Definition
{
    public class ModulesConfig : List<ModuleConfig>
    {
        public void Add(string name, string type)
        {
            Add(new ModuleConfig(name, type));
        }

        public void Add(string name, string type, NameValueCollection parameters)
        {
            Add(new ModuleConfig(name, type, parameters));
        }
    }
}