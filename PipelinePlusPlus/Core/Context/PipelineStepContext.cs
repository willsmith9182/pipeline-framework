using PipelinePlusPlus.Core.Exceptions;

namespace PipelinePlusPlus.Core.Context
{
    public abstract class PipelineStepContext
    {
        protected PipelineStepContext()
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