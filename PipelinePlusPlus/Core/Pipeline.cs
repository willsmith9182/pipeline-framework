using System.Transactions;
using PipelinePlusPlus.Core.Context;

namespace PipelinePlusPlus.Core
{
    internal class Pipeline<TContext> : IPipeline<TContext> where TContext : PipelineStepContext
    {
        public Pipeline(PipelineExecutionContext<TContext> executionContext) { ExecutionContext = executionContext; }
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
                    if (ExecutionContext.CancelExecution)
                    {
                        return new PipelineResult(false, ExecutionContext.Exceptions);
                    }
                }
                pipelineScope.Complete();
            }
            return new PipelineResult(true, ExecutionContext.Exceptions);
        }
    }
}