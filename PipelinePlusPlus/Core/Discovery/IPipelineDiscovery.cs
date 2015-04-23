using PipelinePlusPlus.Core.Context;
using PipelinePlusPlus.Core.Steps;

namespace PipelinePlusPlus.Core.Discovery
{
    internal interface IPipelineDiscovery
    {
        PipelineDefinition<TContext> Discover<TContext>(PipelineSteps pipelineSteps) where TContext : PipelineStepContext;
    }
}