using System;

namespace PipelinePlusPlus.Core.Exceptions
{
    internal class PipelineDicoveryException : Exception
    {
        public PipelineDicoveryException(string message)
            : base(message)
        {
        }
    }
}