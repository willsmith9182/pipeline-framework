using System;

namespace PipelinePlusPlus.Core.Exceptions
{
    [Serializable]
    public sealed class PipelineStepException : Exception
    {
        internal PipelineStepException(string message) : base(message)
        {
        }
    }
}