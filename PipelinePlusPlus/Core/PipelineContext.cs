using System;

namespace PipelinePlusPlus.Core
{
    public abstract class PipelineContext
    {
        internal bool CancelExecution
        {
            get { return CancelReason != null; }
        }

        internal PipelineException CancelReason { get; private set; }

        protected PipelineContext()
        {
            CancelReason = null;
        }

        public void Cancel(string reason, string moduleName)
        {
            CancelReason = new PipelineException(reason, moduleName);
        }
    }
}