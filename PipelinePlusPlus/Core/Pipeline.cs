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
        public Pipeline(PipelineDefninition<TContext> def)
        {
            PipelineDefinintion = def;
            Actions = new Queue<PipelineStepDefinintion<TContext>>(def.Actions);
        }

        internal EventHandler<PipelineEventFiredEventArgs> PipelineStageExecuted { get; set; }
        internal EventHandler<PipelineEventFiringEventArgs> PipelineStageExecuting { get; set; }
        internal Func<PipelineException, bool> OnError { get; set; }
        internal Queue<PipelineStepDefinintion<TContext>> Actions { get; private set; }
        internal PipelineDefninition<TContext> PipelineDefinintion { get; private set; }

        public PipelineResult Execute(TContext context)
        {
            context.OnException = OnError;
            using (var pipelineScope = new TransactionScope(PipelineDefinintion.PipelineScopeOption))
            {
                while (Actions.Count > 0)
                {
                    var pipelineAction = Actions.Dequeue();
                    if (PipelineStageExecuting != null)
                    {
                        var executingArgs = new PipelineEventFiringEventArgs(PipelineDefinintion.PipelineName,
                            pipelineAction.ActionName);
                        PipelineStageExecuting(this, executingArgs);
                        if (executingArgs.Cancel) continue;
                    }
                    using (var actionScope = new TransactionScope())
                    {
                        foreach (
                            var handler in pipelineAction.Action.GetInvocationList().OfType<PipelineAction<TContext>>())
                        {
                            try
                            {
                                handler(context);
                            }
                            catch (Exception e)
                            {
                                var pipeException = new PipelineException("", e);
                                context.RegisterPipelineError(pipeException);
                            }

                            if (context.CancelExecution) return new PipelineResult(false, context.Exceptions);
                        }
                        actionScope.Complete();
                    }
                    if (PipelineStageExecuted == null) continue;
                    var executedArgs = new PipelineEventFiredEventArgs(PipelineDefinintion.PipelineName,
                        pipelineAction.ActionName);
                    PipelineStageExecuted(this, executedArgs);
                }
                pipelineScope.Complete();
            }
            return new PipelineResult(true, context.Exceptions);
        }
    }
}