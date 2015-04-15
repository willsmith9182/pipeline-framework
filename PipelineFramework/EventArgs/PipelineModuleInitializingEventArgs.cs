namespace Pipeline.EventArgs
{
    public class PipelineModuleInitializingEventArgs : PipelineCancelEventArgsBase
    {
        public PipelineModuleInitializingEventArgs(string pipelineName, string pipelineModuleName)
            : base(pipelineName)
        {
            PipelineModuleName = pipelineModuleName;
        }

        public string PipelineModuleName { get; set; }
    }
}