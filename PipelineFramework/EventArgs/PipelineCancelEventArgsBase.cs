namespace Pipeline.EventArgs
{
    public abstract class PipelineCancelEventArgsBase : PipelineEventArgsBase
    {
        protected PipelineCancelEventArgsBase(string pipelineName)
            : base(pipelineName)
        {
        }

        public bool Cancel { get; set; }
    }
}