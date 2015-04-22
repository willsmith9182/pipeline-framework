using System;
using System.Collections.Specialized;
using PipelinePlusPlus.Core;

namespace PipelineConsumer.Pipeline.Modules
{
    internal class ModuleThatWillBreak : PipelineModule<TestPipeline,TestContext>
    {
        public ModuleThatWillBreak() : base("ModuleThatWillBreak")
        {
        }

        public override void Register(TestPipeline pipeline)
        {
            
        }

        public override void ExecuteModule(TestContext cxt)
        {
            
        }
    }
}