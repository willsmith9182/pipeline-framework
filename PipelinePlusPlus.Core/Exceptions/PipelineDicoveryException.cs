using System;

namespace PipelinePlusPlus.Core.Exceptions
{
    [Serializable]
    public sealed class PipelineDicoveryException : Exception
    {
        internal PipelineDicoveryException(string message) : base(message)
        {
        }
    }
}