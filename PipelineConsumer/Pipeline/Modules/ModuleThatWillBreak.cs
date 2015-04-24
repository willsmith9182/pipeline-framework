using PipelinePlusPlus.Core.Modules;

namespace PipelineConsumer.Pipeline.Modules
{
    internal class ModuleThatWillBreak : PipelineModule<TestPipeline, TestStepContext>
    {
        public ModuleThatWillBreak() : base("ModuleThatWillBreak")
        {
        }

        public override void Register(TestPipeline pipeline)
        {
        }

        public override void ExecuteModule(TestStepContext cxt)
        {
        }
    }
}