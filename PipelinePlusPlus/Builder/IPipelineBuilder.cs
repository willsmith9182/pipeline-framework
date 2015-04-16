using System;
using PipelinePlusPlus.Core;
using PipelinePlusPlus.EventArgs;

namespace PipelinePlusPlus.Builder
{
    // the pipeline definition, 
    // this is where events are registered
    // a implementor of this platform will define this and the event order. 

    public interface IPipelineBuilder<out TPipeline, in TContext>
        where TPipeline : PipelineSteps<TContext>, new()
        where TContext : PipelineContext
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

        IPipelineBuilder<TPipeline, TContext> RegisterModule<T>(T module) where T : IPipelineModule<TPipeline>;
        IPipelineBuilder<TPipeline, TContext> RegisterModule<T>() where T : IPipelineModule<TPipeline>, new();
        IPipeline<TContext> Make();
    }
}