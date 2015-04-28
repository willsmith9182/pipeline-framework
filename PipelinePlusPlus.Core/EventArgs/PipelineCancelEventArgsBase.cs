namespace PipelinePlusPlus.Core.EventArgs
{
    public abstract class PipelineCancelEventArgsBase : PipelineEventArgsBase
    {
        protected PipelineCancelEventArgsBase(string pipelineName) : base(pipelineName)
        {
        }

        public bool Cancel { get; private set; }

        public void CancelAction()
        {
            if (!Cancel)
            {
                Cancel = true;
            }
        }
    }
}