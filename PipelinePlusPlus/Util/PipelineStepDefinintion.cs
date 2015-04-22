using System;
using System.Diagnostics.Contracts;
using System.Transactions;
using PipelinePlusPlus.Attributes;
using PipelinePlusPlus.Core;
using PipelinePlusPlus.EventArgs;

namespace PipelinePlusPlus.Util
{
    public class PipelineStepDefinintion<TContext> : IPipelineStepDefinintion<TContext> where TContext : PipelineContext
    {
        internal PipelineStepDefinintion(string stepName, PipelineStepAttribute attr, PipelineStep<TContext> step)
        {
            Contract.Requires(attr != null);
            Contract.Requires(!string.IsNullOrEmpty(stepName));
            Contract.Requires(step != null)
                ;
            StepName = stepName;
            Step = step;
            Attr = attr;
        }

        internal PipelineStepAttribute Attr { get; private set; }
        internal PipelineStep<TContext> Step { get; private set; }
        public string StepName { get; set; }

        public void Execute(IPipelineExecutionContext<TContext> executionContext)
        {
            // if a handler skips this event?
            if (!FireStageExecutingEvent(executionContext)) return;

            using (var actionScope = new TransactionScope())
            {
                foreach (var module in Step)
                {
                    try
                    {
                        module.ExecuteModule(executionContext.StepContext);
                        if (executionContext.StepContext.CancelExecution) executionContext.CancelCurrentExecution();
                    }
                    catch (Exception e)
                    {
                        var pipeException = new PipelineException("", e);
                        executionContext.RegisterPipelineError(pipeException, module.ModuleName);
                    }

                    if (executionContext.CancelExecution) return;
                }
                actionScope.Complete();
            }
            FireStageExecutedEvent(executionContext);
        }

        private bool FireStageExecutingEvent(IPipelineExecutionContext<TContext> executionContext)
        {
            if (executionContext.PipelineStageExecuting == null) return true;
            var executingArgs = new PipelineEventFiringEventArgs(executionContext.PipelineName, StepName);
            executionContext.PipelineStageExecuting(this, executingArgs);
            return !executingArgs.Cancel;
        }

        private void FireStageExecutedEvent(IPipelineExecutionContext<TContext> executionContext)
        {
            if (executionContext.PipelineStageExecuted == null) return;
            var executedArgs = new PipelineEventFiredEventArgs(executionContext.PipelineName, StepName);
            executionContext.PipelineStageExecuted(this, executedArgs);
        }
    }
}