using PipelinePlusPlus.Core.Context;

namespace PipelinePlusPlus.Core.Modules
{
    public interface IPipelineModule<in TContext> where TContext : PipelineStepContext
    {
        string ModuleName { get; }
        void ExecuteModule(TContext cxt);
    }
}