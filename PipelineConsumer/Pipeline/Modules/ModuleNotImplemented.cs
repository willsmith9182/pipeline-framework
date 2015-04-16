using System;
using System.Collections.Specialized;
using PipelinePlusPlus.Core;

namespace PipelineConsumer.Pipeline.Modules
{
    internal class ModuleNotImplemented : IPipelineModule<TestPipeline>
    {
        public void RegisterModuleWithPipeline(TestPipeline pipeline, NameValueCollection parameters = null)
        {
            pipeline.Step2 += cxt =>
            {
                Console.WriteLine("NotImplemented module running at Step2, time to throw an exception");
                cxt.RegisterPipelineError(new NotImplementedException("i'm not implemneted, i should stop execution"));
            };
        }
    }
}