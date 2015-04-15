using System;
using Pipeline.EventArgs;

namespace Pipeline
{
    public interface IBackbone<out TEvents>
        where TEvents : PipelineEvents, new()
    {
        event EventHandler<PipelineModuleInitializingEventArgs> PipelineModuleInitializing;
        event EventHandler<PipelineModuleInitializedEventArgs> PipelineModuleInitialized;
        TEvents Initialize();
    }
}