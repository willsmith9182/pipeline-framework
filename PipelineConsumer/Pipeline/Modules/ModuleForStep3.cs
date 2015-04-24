using System;
using PipelinePlusPlus.Core.Modules;

namespace PipelineConsumer.Pipeline.Modules
{
    internal class ModuleForStep3 : PipelineModule<TestPipeline, TestStepContext>
    {
        public ModuleForStep3() : base("ModuleForStep3")
        {
        }

        public override void Register(TestPipeline pipeline)
        {
            pipeline.Step3.RegisterModule(this);
        }

        public override void ExecuteModule(TestStepContext cxt)
        {
            Console.WriteLine("Step 3 executed!");
        }
    }
}