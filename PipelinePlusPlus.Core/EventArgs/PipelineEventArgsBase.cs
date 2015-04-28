namespace PipelinePlusPlus.Core.EventArgs
{
    public abstract class PipelineEventArgsBase : System.EventArgs
    {
        protected PipelineEventArgsBase(string pipelineName)
        {
            PipelineName = pipelineName;
        }

        public string PipelineName { get; set; }
    }
}