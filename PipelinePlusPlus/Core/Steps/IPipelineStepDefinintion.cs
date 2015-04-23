using PipelinePlusPlus.Core.Context;

namespace PipelinePlusPlus.Core.Steps
{
    public interface IPipelineStepDefinintion<in TContext> where TContext : PipelineStepContext
    {
        void Execute(IPipelineExecutionContext<TContext> executionContext);
    }
}