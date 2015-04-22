using PipelinePlusPlus.Core;

namespace PipelinePlusPlus.Util
{
    public interface IPipelineStepDefinintion<in TContext> where TContext : PipelineContext
    {
        void Execute(IPipelineExecutionContext<TContext> executionContext);
    }
}