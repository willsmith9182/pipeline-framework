using System.Collections;
using System.Collections.Generic;

namespace PipelinePlusPlus.Core
{
    public sealed class PipelineStep<TContext> : IReadOnlyCollection<IPipelineModule<TContext>>
        where TContext : PipelineContext
    {
        private readonly ICollection<IPipelineModule<TContext>> _registeredModules =
            new List<IPipelineModule<TContext>>();

        internal PipelineStep()
        {
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

        internal void RegisterModule(IPipelineModule<TContext> module)
        {
            _registeredModules.Add(module);
        }
    }
}