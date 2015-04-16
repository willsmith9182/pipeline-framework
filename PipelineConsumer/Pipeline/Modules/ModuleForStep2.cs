using System;
using System.Collections.Specialized;
using PipelinePlusPlus.Core;

namespace PipelineConsumer.Pipeline.Modules
{
    internal class ModuleForStep2 : IPipelineModule<TestPipeline>
    {
        public void RegisterModuleWithPipeline(TestPipeline pipeline, NameValueCollection parameters = null)
        {
            pipeline.Step2 += cxt => { Console.WriteLine("Step two has been fired"); };
        }
    }
}