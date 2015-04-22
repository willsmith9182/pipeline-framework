using System;
using System.Collections.Specialized;
using PipelinePlusPlus.Core;

namespace PipelineConsumer.Pipeline.Modules
{
    internal class ModuleForStep1 : PipelineModule<TestPipeline, TestContext>
    {
        public override void Register(TestPipeline pipeline)
        {
            pipeline.Step1.RegisterModule(this);
        }

        public override void ExecuteModule(TestContext cxt)
        {
            Console.WriteLine("Step one has been fired");
        }

        public ModuleForStep1()
            : base("ModuleForStep1")
        {
        }
    }
}