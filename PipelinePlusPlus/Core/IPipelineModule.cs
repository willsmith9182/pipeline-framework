using System.Collections.Specialized;

namespace PipelinePlusPlus.Core
{
    public interface IPipelineModule<in TPipeline> where TPipeline : PipelineSteps
    {
        void RegisterModuleWithPipeline(TPipeline pipeline, NameValueCollection parameters = null);
    }
}