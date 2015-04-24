using System;
using System.Collections.Generic;
using System.Configuration;
using PipelinePlusPlus.Core.Context;
using PipelinePlusPlus.Core.EventArgs;
using PipelinePlusPlus.Core.Modules;
using PipelinePlusPlus.Core.Steps;

namespace PipelinePlusPlus.Core.Discovery
{
    internal interface IDiscoveryFactory
    {
        IPipelineDiscovery<TPipeline, TContext> GetDiscovery<TPipeline, TContext>(TPipeline pipelineSteps, EventHandler<PipelineModuleInitializedEventArgs> moduleInitializedHandler, EventHandler<PipelineModuleInitializingEventArgs> moduleInitializingHandler) where TPipeline : PipelineSteps where TContext : PipelineStepContext;
    }

    internal interface IPipelineDiscovery<TPipeline, TContext> where TPipeline : PipelineSteps where TContext : PipelineStepContext
    {
        PipelineDefinition<TContext> ResolvePipeline(IEnumerable<PipelineModule<TPipeline, TContext>> modules, Configuration appConfig);
    }

    internal interface IPipelineDiscovery
    {
        PipelineDefinition<TContext> Discover<TContext>(PipelineSteps pipelineSteps) where TContext : PipelineStepContext;
    }
}