using System.Collections.Generic;
using System.Transactions;
using PipelinePlusPlus.Core.Context;
using PipelinePlusPlus.Core.Steps;

namespace PipelinePlusPlus.Core.Discovery
{
    public interface IPipelineDefinition<in TContext> where TContext : PipelineStepContext
    {
        IReadOnlyCollection<IPipelineStepDefinintion<TContext>> Actions { get; }
        TransactionScopeOption PipelineScopeOption { get; }
        string PipelineName { get; }
    }
}