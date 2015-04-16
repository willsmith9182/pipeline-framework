namespace PipelinePlusPlus.Core
{
    public delegate void PipelineAction<in T>(T context) where T : PipelineContext;
}