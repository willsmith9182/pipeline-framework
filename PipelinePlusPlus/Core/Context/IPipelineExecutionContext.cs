using System;
using System.Transactions;
using PipelinePlusPlus.Core.EventArgs;
using PipelinePlusPlus.Core.Exceptions;

namespace PipelinePlusPlus.Core.Context
{
    public interface IPipelineExecutionContext<out TContext> where TContext : PipelineStepContext
    {
        EventHandler<PipelineEventFiredEventArgs> PipelineStageExecuted { get; set; }
        EventHandler<PipelineEventFiringEventArgs> PipelineStageExecuting { get; set; }
        bool CancelExecution { get; }
        TContext StepContext { get; }
        TransactionScopeOption PipelineScope { get; }
        string PipelineName { get; }
        void CancelCurrentExecution();
        void CancelCurrentExecution(PipelineException cancellation);
        void RegisterPipelineError(Exception e, string moduleName);
    }
}