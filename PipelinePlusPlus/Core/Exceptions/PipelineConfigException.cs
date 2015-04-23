using System;

namespace PipelinePlusPlus.Core.Exceptions
{
    public class PipelineConfigException : Exception
    {
        public PipelineConfigException(string message) : base(message) { }
        public PipelineConfigException(string message, Exception innerException) : base(message, innerException) { }
    }
}