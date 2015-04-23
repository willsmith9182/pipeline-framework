﻿using System;
using System.Configuration;
using System.Diagnostics;
using PipelineConsumer.Pipeline;
using PipelineConsumer.Pipeline.Modules;
using PipelinePlusPlus.Builder;

namespace PipelineConsumer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var b = PipelineBuilder.CreatePipeline<TestPipeline, TestStepContext>()
                                   .OnModuleInitialize(a => { Console.WriteLine("initializing module: {0}", a.ModuleName); })
                                   .OnModuleInitialized(a => { Console.WriteLine("module initialized and registered: {0}", a.ModuleName); })
                                   .OnPipeLineStageExectue(a => { Console.WriteLine("executing pipeline stage: {0}", a.PipelineEventName); })
                                   .OnPipeLineStageExectued(a => { Console.WriteLine("executed pipeline stage: {0}", a.PipelineEventName); })
                                   .OnPipelineError(e => !(e.InnerException is NotImplementedException))
                                   .RegisterModule<ModuleForStep1>()
                                   .RegisterModule<ModuleForStep2>()
                                   .RegisterModule<ModuleForStep3>()
                                   .RegisterModule<ModuleNotImplemented>()
                                   .RegisterModule<ModuleThatWillBreak>()
                                   .Make(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None));

            var cxt = new TestStepContext();

            var result = b.Execute(cxt);


#if DEBUG
            Debugger.Break();
#endif
            Console.ReadLine();
        }
    }
}