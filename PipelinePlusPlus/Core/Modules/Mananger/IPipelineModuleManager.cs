using System;
using System.Collections.Generic;
using PipelinePlusPlus.Core.Context;
using PipelinePlusPlus.Core.DynamicConfig;
using PipelinePlusPlus.Core.EventArgs;
using PipelinePlusPlus.Core.Steps;

namespace PipelinePlusPlus.Core.Modules.Mananger
{
    public interface IPipelineModuleManager
    {
        void RegisterDynamicModules<TPipeline, TContext>(
            TPipeline pipeline,
            PipelineDynamicModuleConfig<TContext> pipelineDynamicModuleConfig,
            EventHandler<PipelineModuleInitializingEventArgs> initializingHandler,
            EventHandler<PipelineModuleInitializedEventArgs> initializedHandler)
            where TPipeline : PipelineSteps
            where TContext : PipelineStepContext;

        void RegisterModules<TPipeline, TContext>(
            TPipeline pipeline,
            IEnumerable<PipelineModule<TPipeline, TContext>> modules,
            EventHandler<PipelineModuleInitializingEventArgs> initializingHandler,
            EventHandler<PipelineModuleInitializedEventArgs> initializedHandler)
            where TPipeline : PipelineSteps
            where TContext : PipelineStepContext;
    }
}