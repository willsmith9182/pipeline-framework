using System.Collections.Specialized;

namespace PipelinePlusPlus.Definition
{
    public class ModuleConfig
    {
        public ModuleConfig(string name, string type, NameValueCollection parameters)
        {
            Name = name;
            Type = type;
            Parameters = parameters;
        }

        public string Name { get; set; }
        public string Type { get; set; }
        public NameValueCollection Parameters { get; set; }
    }
}