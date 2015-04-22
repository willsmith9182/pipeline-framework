using System;

namespace PipelinePlusPlus.Util
{
    internal class PipelineDicoveryException : Exception
    {
        public PipelineDicoveryException(string message)
            : base(message)
        {
        }
    }
}