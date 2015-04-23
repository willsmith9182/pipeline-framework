using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Transactions;
using PipelinePlusPlus.Core.Context;
using PipelinePlusPlus.Core.Steps;

namespace PipelinePlusPlus.Core.Discovery
{
    public class PipelineDefinition<TContext> where TContext : PipelineStepContext
    {
        internal PipelineDefinition(IEnumerable<IPipelineStepDefinintion<TContext>> actions,
            TransactionScopeOption pipelineScopeOption, string pipelineName)
        {
            Actions = new ReadOnlyCollection<IPipelineStepDefinintion<TContext>>(actions.ToList());
            PipelineScopeOption = pipelineScopeOption;
            PipelineName = pipelineName;
        }

        public IReadOnlyCollection<IPipelineStepDefinintion<TContext>> Actions { get; private set; }
        public TransactionScopeOption PipelineScopeOption { get; private set; }
        public string PipelineName { get; private set; }
    }
}