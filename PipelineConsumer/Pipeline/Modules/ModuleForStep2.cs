using System;
using PipelinePlusPlus.Core.Modules;

namespace PipelineConsumer.Pipeline.Modules
{
    internal class ModuleForStep2 : PipelineModule<TestPipeline, TestStepContext>
    {
        public ModuleForStep2() : base("ModuleForStep2")
        {
        }

        public override void Register(TestPipeline pipeline)
        {
            pipeline.Step2.RegisterModule(this);
        }

        public override void ExecuteModule(TestStepContext cxt)
        {
            Console.WriteLine("Step two has been fired");
        }
    }
}