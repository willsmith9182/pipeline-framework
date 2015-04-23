using System.Collections.Specialized;
using PipelinePlusPlus.Core.Context;
using PipelinePlusPlus.Core.Steps;

namespace PipelinePlusPlus.Core.Modules
{
    public abstract class DynamicPipelineModule<TPipeline, TContext> : PipelineModule<TPipeline, TContext> where TContext : PipelineStepContext where TPipeline : PipelineSteps
    {
        protected DynamicPipelineModule(string moduleName) : base(moduleName) { }
        public override void Register(TPipeline pipeline) { Register(pipeline, new NameValueCollection()); }
        public abstract void Register(TPipeline pipeline, NameValueCollection parameters);
    }
}