namespace PipelinePlusPlus.Core
{
    public interface IPipelineModule<in TContext>
        where TContext : PipelineContext
    {
        string ModuleName { get; }
        void ExecuteModule(TContext cxt);
    }
}