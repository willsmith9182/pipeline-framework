using System;
using System.Collections.Specialized;
using PipelinePlusPlus.Core;

namespace PipelineConsumer.Pipeline.Modules
{
    internal class ModuleForStep3 : PipelineModule<TestPipeline,TestContext>
    {
        public ModuleForStep3()
            : base("ModuleForStep3")
        {
        }

        public override void Register(TestPipeline pipeline)
        {
            pipeline.Step3.RegisterModule(this);
        }

        public override void ExecuteModule(TestContext cxt)
        {
            Console.WriteLine("Step 3 executed!");
        }
    }
}