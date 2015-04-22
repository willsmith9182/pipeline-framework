using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using PipelinePlusPlus.EventArgs;
using PipelinePlusPlus.Util;

namespace PipelinePlusPlus.Core
{
    public class PipelineResult
    {
        public PipelineResult(bool completed, IReadOnlyCollection<PipelineException> exceptions)
        {
            Completed = completed;
            Errored = !completed;
            if (!exceptions.Any()) return;
            var msg = completed
                ? "Pipeline ran to completion but errors were raised during the execution"
                : "Pipeline was unable to complete due to exceptions encountered during execution";
            ExecutionException = new AggregateException(msg, exceptions);
        }

        public AggregateException ExecutionException { get; private set; }
        public bool Completed { get; private set; }
        public bool Errored { get; private set; }
    }

    internal class Pipeline<TContext> : IPipeline<TContext>
        where TContext : PipelineContext
    {
        public Pipeline(PipelineExecutionContext<TContext> executionContext)
        {
            ExecutionContext = executionContext;
        }

        public PipelineExecutionContext<TContext> ExecutionContext { get; private set; }
        
        public PipelineResult Execute(TContext context)
        {
            // set the context for this execution
            // resets the execution context
            ExecutionContext.StepContext = context;

            using (var pipelineScope = new TransactionScope(ExecutionContext.PipelineScope))
            {
                foreach (var step in ExecutionContext.Steps)
                {
                    step.Execute(ExecutionContext);
                    if (ExecutionContext.CancelExecution) return new PipelineResult(false, ExecutionContext.Exceptions);
                }
                pipelineScope.Complete();
            }
            return new PipelineResult(true, ExecutionContext.Exceptions);
        }
    }
}