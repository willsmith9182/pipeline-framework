using PipelinePlusPlus.Core.Context;

namespace PipelinePlusPlus.Core
{
    public interface IPipeline<in TContext> where TContext : PipelineStepContext
    {
        PipelineResult Execute(TContext context);
    }
}