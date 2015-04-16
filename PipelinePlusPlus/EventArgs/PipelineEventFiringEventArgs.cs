namespace PipelinePlusPlus.EventArgs
{
    public class PipelineExceptionEventArgs
    {
    }

    public class PipelineEventFiringEventArgs : PipelineCancelEventArgsBase
    {
        public PipelineEventFiringEventArgs(string pipelineName)
            : this(pipelineName, "")
        {
        }

        public PipelineEventFiringEventArgs(string pipelineName, string pipelineEventName)
            : base(pipelineName)
        {
            PipelineEventName = pipelineEventName;
        }

        public string PipelineEventName { get; set; }
    }
}