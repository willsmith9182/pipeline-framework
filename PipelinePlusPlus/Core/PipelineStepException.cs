using System;

namespace PipelinePlusPlus.Core
{
    public sealed class PipelineStepException : Exception
    {
        public PipelineStepException(string message)
            : base(message)
        {
        }
    }
}