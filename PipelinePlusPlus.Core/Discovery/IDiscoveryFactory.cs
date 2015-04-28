using System;
using PipelinePlusPlus.Core.Context;
using PipelinePlusPlus.Core.EventArgs;
using PipelinePlusPlus.Core.Steps;

namespace PipelinePlusPlus.Core.Discovery
{
    internal interface IDiscoveryFactory
    {
        IPipelineDiscovery<TPipeline, TContext> GetDiscovery<TPipeline, TContext>(TPipeline pipelineSteps, EventHandler<PipelineModuleInitializedEventArgs> moduleInitializedHandler, EventHandler<PipelineModuleInitializingEventArgs> moduleInitializingHandler) where TPipeline : PipelineSteps where TContext : PipelineStepContext;
    }
}