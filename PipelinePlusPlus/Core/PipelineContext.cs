using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PipelinePlusPlus.Definition;

namespace PipelinePlusPlus.Core
{
    public abstract class PipelineContext
    {
        private readonly Collection<PipelineException> _exceptions = new Collection<PipelineException>();

        protected PipelineContext()
        {
            OnException = e => true;
        }

        internal Func<PipelineException, bool> OnException { get; set; }
        internal bool CancelExecution { get; private set; }

        internal IReadOnlyCollection<PipelineException> Exceptions
        {
            get { return _exceptions; }
        }

        public void Cancel(string reason)//, ModuleConfig executingModule)
        {
            _exceptions.Add(new PipelineException(reason, String.Empty)); //executingModule.Type));
            CancelExecution = true;
        }

        public void RegisterPipelineError(Exception e)//, ModuleConfig executingModule)
        {
            var ex = new PipelineException(String.Empty, e); //executingModule.Type, e);
            _exceptions.Add(ex);
            CancelExecution = OnException(ex);
        }
    }
}