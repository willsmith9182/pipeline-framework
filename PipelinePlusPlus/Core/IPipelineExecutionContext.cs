using System;
using System.Transactions;
using PipelinePlusPlus.EventArgs;

namespace PipelinePlusPlus.Core
{
    public interface IPipelineExecutionContext<out TContext> where TContext : PipelineContext
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