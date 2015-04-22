using System;
using PipelinePlusPlus.Core;

namespace PipelineConsumer.Pipeline.Modules
{
    internal class ModuleNotImplemented : PipelineModule<TestPipeline, TestContext>
    {
        public ModuleNotImplemented()
            : base("ModuleNotImplemented")
        {
        }

        public override void Register(TestPipeline pipeline)
        {
            pipeline.Step2.RegisterModule(this);
        }

        public override void ExecuteModule(TestContext cxt)
        {
            Console.WriteLine("NotImplemented module running at Step2, time to throw an exception");
            throw new NotImplementedException("i'm not implemneted, i should stop execution");
        }
    }
}