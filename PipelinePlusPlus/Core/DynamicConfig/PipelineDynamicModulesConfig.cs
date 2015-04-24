namespace PipelinePlusPlus.Core.DynamicConfig
{
    public class PipelineDynamicModuleConfig
    {
        public PipelineDynamicModuleConfig(string name)
        {
            Name = name;
            Modules = new ModulesConfig();
        }

        public string Name { get; private set; }
        public ModulesConfig Modules { get; internal set; }
    }
}