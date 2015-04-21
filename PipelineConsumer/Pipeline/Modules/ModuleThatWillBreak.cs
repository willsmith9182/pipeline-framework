using System;
using System.Collections.Specialized;
using PipelinePlusPlus.Core;

namespace PipelineConsumer.Pipeline.Modules
{
    internal class ModuleThatWillBreak : IPipelineModule<TestPipeline>
    {
        public void RegisterModuleWithPipeline(TestPipeline pipeline, NameValueCollection parameters = null)
        {
            //pipeline.Step2 += cxt =>
            //{
            //    Console.WriteLine("Breaking module running at Step2, time to throw an exception");
            //    cxt.RegisterPipelineError(new Exception("Something when bang."));
            //};
        }
    }
}