namespace Pipeline.EventArgs
{
    public class PipelineModuleInitializedEventArgs : PipelineEventArgsBase
    {
        public PipelineModuleInitializedEventArgs(string pipelineName, string pipelineModuleName)
            : base(pipelineName)
        {
            PipelineModuleName = pipelineModuleName;
        }

        public string PipelineModuleName { get; set; }
    }
}