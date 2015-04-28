using System;

namespace PipelinePlusPlus.Core.Exceptions
{
    [Serializable]
    public class PipelineConfigException : Exception
    {
        internal PipelineConfigException(string message) : base(message)
        {
        }

        internal PipelineConfigException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}