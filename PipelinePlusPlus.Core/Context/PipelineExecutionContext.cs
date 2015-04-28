using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Transactions;
using PipelinePlusPlus.Core.Discovery;
using PipelinePlusPlus.Core.EventArgs;
using PipelinePlusPlus.Core.Exceptions;
using PipelinePlusPlus.Core.Steps;

namespace PipelinePlusPlus.Core.Context
{
    public class PipelineExecutionContext<TContext> : IPipelineExecutionContext<TContext> where TContext : PipelineStepContext
    {
        private readonly IPipelineDefinition<TContext> _definition;
        private readonly Collection<PipelineException> _exceptions;
        private readonly Func<PipelineException, bool> _onException;
        private TContext _stepContext;

        public PipelineExecutionContext(IPipelineDefinition<TContext> defninition, Func<PipelineException, bool> onException = null)
        {
            _definition = defninition;
            _onException = onException ?? (e => true);
            CancelExecution = false;
            _exceptions = new Collection<PipelineException>();
        }

        public IReadOnlyCollection<PipelineException> Exceptions
        {
            get { return _exceptions; }
        }

        public IReadOnlyCollection<IPipelineStepDefinintion<TContext>> Steps
        {
            get { return _definition.Actions; }
        }

        public EventHandler<PipelineEventFiredEventArgs> PipelineStageExecuted { get; internal set; }
        public EventHandler<PipelineEventFiringEventArgs> PipelineStageExecuting { get; internal set; }
        public bool CancelExecution { get; private set; }

        public TContext StepContext
        {
            get { return _stepContext; }
            internal set { ResetContext(value); }
        }

        public TransactionScopeOption PipelineScope
        {
            get { return _definition.PipelineScopeOption; }
        }

        public string PipelineName
        {
            get { return _definition.PipelineName; }
        }

        public void CancelCurrentExecution()
        {
            CancelCurrentExecution(null);
        }

        public void CancelCurrentExecution(PipelineException cancellation)
        {
            CancelExecution = true;
            if (cancellation != null)
            {
                _exceptions.Add(cancellation);
            }
        }

        public void RegisterPipelineError(Exception e, string moduleName)
        {
            var ex = new PipelineException(moduleName, e);
            _exceptions.Add(ex);
            if (!CancelExecution && _onException(ex)) CancelExecution = true;
        }

        private void ResetContext(TContext newContext)
        {
            _stepContext = newContext;
            _exceptions.Clear();
            CancelExecution = false;
        }
    }
}