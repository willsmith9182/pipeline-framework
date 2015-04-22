using System.Collections.Specialized;

namespace PipelinePlusPlus.Core
{
    public interface IPipelineModule<in TContext>
        where TContext : PipelineContext
    {
        string ModuleName { get; }
        void ExecuteModule(TContext cxt);
    }


    public abstract class PipelineModule<TPipeline, TContext> : IPipelineModule<TContext>
        where TContext : PipelineContext
        where TPipeline : PipelineSteps
    {
        protected PipelineModule(string moduleName)
        {
            ModuleName = moduleName;
        }


        public abstract void Register(TPipeline pipeline);

        public string ModuleName { get; private set; }

        public abstract void ExecuteModule(TContext cxt);
    }

    public abstract class DynamicPipelineModule<TPipeline, TContext> : PipelineModule<TPipeline, TContext>
        where TContext : PipelineContext
        where TPipeline : PipelineSteps
    {
        public override void Register(TPipeline pipeline)
        {
            Register(pipeline, new NameValueCollection());
        }

        public abstract void Register(TPipeline pipeline, NameValueCollection parameters);

        protected DynamicPipelineModule(string moduleName)
            : base(moduleName)
        {
        }
    }

}