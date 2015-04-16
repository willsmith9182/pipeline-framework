namespace PipelinePlusPlus.Core
{
    public abstract class PipelineSteps
    {
        public string PipelineName { get; protected set; }
    }

    public abstract class PipelineSteps<TContext> : PipelineSteps
    {
        protected PipelineSteps(string pipelineName)
        {
            PipelineName = pipelineName;
        }

        protected TContext Context { get; private set; }

        internal void Registercontext(TContext context)
        {
            Context = context;
        }
    }
}