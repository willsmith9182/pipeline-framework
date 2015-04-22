namespace PipelinePlusPlus.Core
{
    public abstract class PipelineModule<TPipeline, TContext> : IPipelineModule<TContext>
        where TContext : PipelineContext
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