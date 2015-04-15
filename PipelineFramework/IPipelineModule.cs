using System.Collections.Specialized;

namespace Pipeline
{
    public interface IPipelineModule<in T> where T : PipelineEvents
    {
        void Initialize(T events, NameValueCollection parameters);
    }
}