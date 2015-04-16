using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Transactions;
using PipelinePlusPlus.Core;

namespace PipelinePlusPlus.Util
{
    public class PipelineDefninition<TContext> where TContext : PipelineContext
    {
        internal PipelineDefninition(IList<PipelineStepDefinintion<TContext>> actions,
            TransactionScopeOption pipelineScopeOption, string pipelineName)
        {
            Actions = new Collection<PipelineStepDefinintion<TContext>>(actions);
            PipelineScopeOption = pipelineScopeOption;
            PipelineName = pipelineName;
        }

        public IReadOnlyCollection<PipelineStepDefinintion<TContext>> Actions { get; private set; }
        public TransactionScopeOption PipelineScopeOption { get; private set; }
        public string PipelineName { get; private set; }
    }
}