using System;

namespace PipelinePlusPlus.Core.Exceptions
{
    [Serializable]
    public sealed class PipelineException : Exception
    {
        internal PipelineException(string reason, string moduleName) : base(string.Format("Pipeline cancelled by module: {0}. Module '{1}'.", reason, moduleName))
        {
        }

        internal PipelineException(string moduleName, Exception innerException) : base(string.Format("ERROR when executing pipeline module '{0}'. pipeline will terminate.", moduleName), innerException)
        {
        }
    }
}