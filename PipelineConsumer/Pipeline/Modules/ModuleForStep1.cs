using System;
using System.Collections.Specialized;
using PipelinePlusPlus.Core;

namespace PipelineConsumer.Pipeline.Modules
{
    internal class ModuleForStep1 : IPipelineModule<TestPipeline>
    {
        public void RegisterModuleWithPipeline(TestPipeline pipeline, NameValueCollection parameters = null)
        {
            pipeline.Step1 += cxt =>
            {
                Console.WriteLine("Step one has been fired");
                
                cxt.Cancel("string");
            };
        }
    }
}