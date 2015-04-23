using System;

namespace PipelinePlusPlus.Core.Exceptions
{
    public sealed class PipelineException : Exception
    {
        public PipelineException(string reason, string moduleName) : base(string.Format("Pipeline cancelled by module: {0}. Module '{1}'.", reason, moduleName)) { }
        public PipelineException(string moduleName, Exception innerException) : base(string.Format("ERROR when executing pipeline module '{0}'. pipeline will terminate.", moduleName), innerException) { }
    }
}