using System;
using System.Collections.Specialized;
using PipelinePlusPlus.Core;

namespace PipelineConsumer.Pipeline.Modules
{
    internal class ModuleForStep3 : IPipelineModule<TestPipeline>
    {
        public void RegisterModuleWithPipeline(TestPipeline pipeline, NameValueCollection parameters = null)
        {
            pipeline.Step3 += cxt => { Console.WriteLine("Step three has been fired"); };
        }
    }
}