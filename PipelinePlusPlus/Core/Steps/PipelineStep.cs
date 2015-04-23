using System.Collections;
using System.Collections.Generic;
using PipelinePlusPlus.Core.Context;
using PipelinePlusPlus.Core.Modules;

namespace PipelinePlusPlus.Core.Steps
{
    public sealed class PipelineStep<TContext> : IReadOnlyCollection<IPipelineModule<TContext>> where TContext : PipelineStepContext
    {
        private readonly ICollection<IPipelineModule<TContext>> _registeredModules = new List<IPipelineModule<TContext>>();
        internal PipelineStep() { }
        public IEnumerator<IPipelineModule<TContext>> GetEnumerator() { return _registeredModules.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public int Count { get { return _registeredModules.Count; } }
        public void RegisterModule(IPipelineModule<TContext> module) { _registeredModules.Add(module); }
    }
}