using System;
using Pipeline.EventArgs;
using Pipeline.Support_Code;

namespace Pipeline
{
    public interface IBackbone<TEvents, TContext> : IBackbone<TEvents>
        where TEvents : PipelineEvents, new()
        where TContext : PipelineContext
    {
        event EventHandler<PipelineEventFiringEventArgs> PipelineEventFiring;
        event EventHandler<PipelineEventFiredEventArgs> PipelineEventFired;
        void Execute(PipelineContext<TContext> pipelineEvent, TContext context);
        void Execute(PipelineContext<TContext> pipelineEvent, TContext context, TransactionScopeOption transactionScope);
        void Execute(TEvents pipelineEvents, TContext context);
    }
}