using System;

namespace PipelinePlusPlus.Core.Exceptions
{
    public sealed class PipelineStepException : Exception
    {
        public PipelineStepException(string message)
            : base(message)
        {
        }
    }
}