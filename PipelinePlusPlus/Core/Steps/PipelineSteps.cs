namespace PipelinePlusPlus.Core.Steps
{
    public abstract class PipelineSteps
    {
        protected PipelineSteps(string pipelineName)
        {
            PipelineName = pipelineName;
        }

        public string PipelineName { get; protected set; }
    }
}