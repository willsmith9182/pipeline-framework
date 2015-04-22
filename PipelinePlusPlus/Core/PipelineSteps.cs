using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Transactions;
using PipelinePlusPlus.EventArgs;
using PipelinePlusPlus.Util;

namespace PipelinePlusPlus.Core
{
    public abstract class PipelineSteps
    {
        public string PipelineName { get; protected set; }

        protected PipelineSteps(string pipelineName)
        {
            PipelineName = pipelineName;
        }
    }

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

    public class PipelineExecutionContext<TContext> : IPipelineExecutionContext<TContext> where TContext : PipelineContext
    {
        public PipelineExecutionContext(PipelineDefinition<TContext> defninition, Func<PipelineException, bool> onException = null)
        {
            _definition = defninition;
            _onException = onException ?? (e => true);
            CancelExecution = false;
            _exceptions = new Collection<PipelineException>();
        }

        private readonly Collection<PipelineException> _exceptions;
        private readonly PipelineDefinition<TContext> _definition;
        public EventHandler<PipelineEventFiredEventArgs> PipelineStageExecuted { get; set; }
        public EventHandler<PipelineEventFiringEventArgs> PipelineStageExecuting { get; set; }

        public bool CancelExecution { get; private set; }

        private TContext _stepContext;

        public TContext StepContext
        {
            get { return _stepContext; }
            internal set { ResetContext(value); }
        }
        public TransactionScopeOption PipelineScope { get { return _definition.PipelineScopeOption; } }
        public IReadOnlyCollection<PipelineException> Exceptions { get { return _exceptions; } }
        public string PipelineName { get { return _definition.PipelineName; } }
        public IReadOnlyCollection<IPipelineStepDefinintion<TContext>> Steps { get { return _definition.Actions; } }
        private readonly Func<PipelineException, bool> _onException;


        private void ResetContext(TContext newContext)
        {
            _stepContext = newContext;
            _exceptions.Clear();
            CancelExecution = false;
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
            CancelExecution = _onException(ex);
        }
    }

    public static class Ext
    {
        // this means that people don't need to instansiate the Step before they try and register modules. 
        // infact they can't the ctor is internal to prevent anyone creating steps. they jsut define, the pipeline will take care of the rest. 
        // this'll look like an instance method but it'll create the instance if it doesn't exist. 
        public static void RegisterModule<TContext>(this PipelineStep<TContext> step, IPipelineModule<TContext> module) where TContext : PipelineContext
        {
            if (step == null)
            {
                step = new PipelineStep<TContext>();
            }
            step.RegisterModule(module);
        }
    }

    public sealed class PipelineStep<TContext> : IReadOnlyCollection<IPipelineModule<TContext>> where TContext : PipelineContext
    {
        private readonly ICollection<IPipelineModule<TContext>> _registeredModules = new List<IPipelineModule<TContext>>();

        internal PipelineStep()
        {

        }

        internal void RegisterModule(IPipelineModule<TContext> module)
        {
            _registeredModules.Add(module);
        }

        public IEnumerator<IPipelineModule<TContext>> GetEnumerator()
        {
            return _registeredModules.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count
        {
            get { return _registeredModules.Count; }
        }

    }
}