namespace PipelinePlusPlus.Core
{
    public interface IPipeline<in TContext> where TContext : PipelineContext
    {
        PipelineResult Execute(TContext context);
    }
}