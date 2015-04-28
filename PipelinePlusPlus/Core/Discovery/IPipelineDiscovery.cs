using System.Collections.Generic;
using System.Configuration;
using PipelinePlusPlus.Core.Context;
using PipelinePlusPlus.Core.Modules;
using PipelinePlusPlus.Core.Steps;

namespace PipelinePlusPlus.Core.Discovery
{
    internal interface IPipelineDiscovery<TPipeline, TContext> where TPipeline : PipelineSteps where TContext : PipelineStepContext
    {
        IPipelineDefinition<TContext> ResolvePipeline(IEnumerable<PipelineModule<TPipeline, TContext>> modules, Configuration appConfig);
    }
}