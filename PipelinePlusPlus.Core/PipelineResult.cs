using System;
using System.Collections.Generic;
using System.Linq;
using PipelinePlusPlus.Core.Exceptions;

namespace PipelinePlusPlus.Core
{
    public class PipelineResult
    {
        public PipelineResult(bool completed, IReadOnlyCollection<PipelineException> exceptions)
        {
            Completed = completed;
            Errored = !completed;
            if (!exceptions.Any())
            {
                return;
            }
            var msg = completed ? "Pipeline ran to completion but errors were raised during the execution" : "Pipeline was unable to complete due to exceptions encountered during execution";
            ExecutionException = new AggregateException(msg, exceptions);
        }

        public AggregateException ExecutionException { get; private set; }
        public bool Completed { get; private set; }
        public bool Errored { get; private set; }
    }
}