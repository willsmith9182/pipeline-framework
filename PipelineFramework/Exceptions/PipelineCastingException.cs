using System;

namespace Pipeline.Exceptions
{
    public class PipelineCastingException : Exception
    {
        public PipelineCastingException()
        {
        }

        public PipelineCastingException(string message) : base(message)
        {
        }

        public PipelineCastingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}