namespace PipelinePlusPlus.Core
{
    public abstract class PipelineContext
    {
        protected PipelineContext()
        {
            CancelReason = null;
        }

        internal bool CancelExecution
        {
            get { return CancelReason != null; }
        }

        internal PipelineException CancelReason { get; private set; }

        public void Cancel(string reason, string moduleName)
        {
            CancelReason = new PipelineException(reason, moduleName);
        }
    }
}