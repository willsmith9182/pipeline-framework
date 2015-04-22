using PipelinePlusPlus.Core;

namespace PipelinePlusPlus.Util
{
    internal interface IPipelineDiscovery
    {
        PipelineDefinition<TContext> Discover<TContext>(PipelineSteps pipelineSteps) where TContext : PipelineContext;
    }
}