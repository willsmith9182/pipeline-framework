﻿using System;
using System.Configuration;
using PipelinePlusPlus.Core;
using PipelinePlusPlus.Core.Context;
using PipelinePlusPlus.Core.EventArgs;
using PipelinePlusPlus.Core.Exceptions;
using PipelinePlusPlus.Core.Modules;
using PipelinePlusPlus.Core.Steps;

namespace PipelinePlusPlus.Builder
{
    // the pipeline definition, 
    // this is where events are registered
    // a implementor of this platform will define this and the event order. 

    public interface IPipelineBuilder<TPipeline, TContext> where TPipeline : PipelineSteps, new() where TContext : PipelineStepContext
    {
        IPipelineBuilder<TPipeline, TContext> OnModuleInitialize(Action<object, PipelineModuleInitializingEventArgs> del);
        IPipelineBuilder<TPipeline, TContext> OnModuleInitialized(Action<object, PipelineModuleInitializedEventArgs> del);
        IPipelineBuilder<TPipeline, TContext> OnModuleInitialize(Action<PipelineModuleInitializingEventArgs> del);
        IPipelineBuilder<TPipeline, TContext> OnModuleInitialized(Action<PipelineModuleInitializedEventArgs> del);
        IPipelineBuilder<TPipeline, TContext> OnPipeLineStageExectue(Action<object, PipelineEventFiringEventArgs> del);
        IPipelineBuilder<TPipeline, TContext> OnPipeLineStageExectue(Action<PipelineEventFiringEventArgs> del);
        IPipelineBuilder<TPipeline, TContext> OnPipeLineStageExectued(Action<object, PipelineEventFiredEventArgs> del);
        IPipelineBuilder<TPipeline, TContext> OnPipeLineStageExectued(Action<PipelineEventFiredEventArgs> del);

        /// <summary>
        ///     Adds a delegate to the pipeline that is called each time an error is encountered.
        ///     allows the pipeline to be configured to ignore certain/all errors.
        /// </summary>
        /// <param name="del">
        ///     a delegate that takes a PipelineException and returns a boolean to indicate if the pipeline should
        ///     halt or carry on
        /// </param>
        /// <returns></returns>
        IPipelineBuilder<TPipeline, TContext> OnPipelineError(Func<PipelineException, bool> del);

        IPipelineBuilder<TPipeline, TContext> RegisterModule<T>(T module) where T : PipelineModule<TPipeline, TContext>;
        IPipelineBuilder<TPipeline, TContext> RegisterModule<T>() where T : PipelineModule<TPipeline, TContext>, new();
        IPipeline<TContext> Make(Configuration appConfig);
    }
}