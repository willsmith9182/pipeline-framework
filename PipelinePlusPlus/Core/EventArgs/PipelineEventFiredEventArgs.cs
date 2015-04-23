namespace PipelinePlusPlus.Core.EventArgs
{
    public class PipelineEventFiredEventArgs : PipelineEventArgsBase
    {
        public PipelineEventFiredEventArgs(string pipelineName) : this(pipelineName, "") { }
        public PipelineEventFiredEventArgs(string pipelineName, string pipelineEventName) : base(pipelineName) { PipelineEventName = pipelineEventName; }
        public string PipelineEventName { get; set; }
    }
}