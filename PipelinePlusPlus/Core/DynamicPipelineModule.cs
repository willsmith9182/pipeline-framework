using System.Collections.Specialized;

namespace PipelinePlusPlus.Core
{
    public abstract class DynamicPipelineModule<TPipeline, TContext> : PipelineModule<TPipeline, TContext>
        where TContext : PipelineContext
        where TPipeline : PipelineSteps
    {
        protected DynamicPipelineModule(string moduleName)
            : base(moduleName)
        {
        }

        public override void Register(TPipeline pipeline)
        {
            Register(pipeline, new NameValueCollection());
        }

        public abstract void Register(TPipeline pipeline, NameValueCollection parameters);
    }
}