using PipelinePlusPlus.Core.Context;
using PipelinePlusPlus.Core.Steps;

namespace PipelinePlusPlus.Core.Modules
{
    public abstract class PipelineModule<TPipeline, TContext> : IPipelineModule<TContext>
        where TContext : PipelineStepContext
        where TPipeline : PipelineSteps
    {
        protected PipelineModule(string moduleName)
        {
            ModuleName = moduleName;
        }

        public string ModuleName { get; private set; }
        public abstract void ExecuteModule(TContext cxt);
        public abstract void Register(TPipeline pipeline);
    }
}