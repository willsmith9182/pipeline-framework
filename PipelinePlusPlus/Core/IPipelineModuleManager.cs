using System;
using System.Collections.Generic;
using PipelinePlusPlus.Definition;
using PipelinePlusPlus.EventArgs;

namespace PipelinePlusPlus.Core
{
    public interface IPipelineModuleManager
    {
        void RegisterDynamicModules<TPipeline, TContext>(
            TPipeline pipeline,
            PipelineConfig<TContext> pipelineConfig,
            EventHandler<PipelineModuleInitializingEventArgs> initializingHandler,
            EventHandler<PipelineModuleInitializedEventArgs> initializedHandler)
            where TPipeline : PipelineSteps
            where TContext : PipelineContext;

        void RegisterModules<TPipeline, TContext>(
            TPipeline pipeline,
            IEnumerable<PipelineModule<TPipeline, TContext>> modules,
            EventHandler<PipelineModuleInitializingEventArgs> initializingHandler,
            EventHandler<PipelineModuleInitializedEventArgs> initializedHandler)
            where TPipeline : PipelineSteps
            where TContext : PipelineContext;
    }
}