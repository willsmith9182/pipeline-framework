using System;
using PipelinePlusPlus.Core.Modules;

namespace PipelineConsumer.Pipeline.Modules
{
    internal class ModuleForStep1 : PipelineModule<TestPipeline, TestStepContext>
    {
        public ModuleForStep1() : base("ModuleForStep1") { }
        public override void Register(TestPipeline pipeline) { pipeline.Step1.RegisterModule(this); }
        public override void ExecuteModule(TestStepContext cxt) { Console.WriteLine("Step one has been fired"); }
    }
}