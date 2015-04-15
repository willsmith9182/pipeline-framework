namespace Pipeline.Support_Code
{
    public delegate void PipelineContext<in T>(T context) where T : PipelineContext;
}