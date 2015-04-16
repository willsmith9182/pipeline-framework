using System.Collections.Specialized;

namespace Pipeline
{
    public interface IPipelineModule<in TPipeline> where TPipeline : Pipeline
    {
        void Initialize(TPipeline pipeline, NameValueCollection parameters);
    }
}