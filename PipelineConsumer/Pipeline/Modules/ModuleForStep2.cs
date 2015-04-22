using System;
using System.Collections.Specialized;
using PipelinePlusPlus.Core;

namespace PipelineConsumer.Pipeline.Modules
{
    internal class ModuleForStep2 : PipelineModule<TestPipeline, TestContext>
    {

        public ModuleForStep2() : base("ModuleForStep2")
        {
        }

        public override void Register(TestPipeline pipeline)
        {
            pipeline.Step2.RegisterModule(this);
        }

        public override void ExecuteModule(TestContext cxt)
        {
            Console.WriteLine("Step two has been fired");
        }
    }
}