using System;
using System.Collections.Generic;
using PipelinePlusPlus.Core;
using PipelinePlusPlus.Definition;
using PipelinePlusPlus.EventArgs;
using PipelinePlusPlus.Util;

namespace PipelinePlusPlus.Builder
{
    public class PipelineBuilder<TPipeline, TContext> : IPipelineBuilder<TPipeline, TContext>
        where TPipeline : PipelineSteps<TContext>, new()
        where TContext : PipelineContext
    {
        private readonly ICollection<IPipelineModule<TPipeline>> _modules = new List<IPipelineModule<TPipeline>>();
        private readonly TPipeline _pipelineSteps;
        private EventHandler<PipelineModuleInitializedEventArgs> _moduleInitalized;
        private EventHandler<PipelineModuleInitializingEventArgs> _moduleInitalizing;
        private Func<PipelineException, bool> _onError = e => true;
        private EventHandler<PipelineEventFiredEventArgs> _pipelineStageExecuted;
        private EventHandler<PipelineEventFiringEventArgs> _pipelineStageExecuting;

        internal PipelineBuilder()
        {
            _pipelineSteps = new TPipeline();
        }

        public IPipelineBuilder<TPipeline, TContext> OnModuleInitialize(
            Action<object, PipelineModuleInitializingEventArgs> del)
        {
            return AddHandler((o, ea) => del(o, ea));
        }

        public IPipelineBuilder<TPipeline, TContext> OnModuleInitialized(
            Action<object, PipelineModuleInitializedEventArgs> del)
        {
            return AddHandler((o, ea) => del(o, ea));
        }

        public IPipelineBuilder<TPipeline, TContext> OnModuleInitialize(Action<PipelineModuleInitializingEventArgs> del)
        {
            return AddHandler((o, ea) => del(ea));
        }

        public IPipelineBuilder<TPipeline, TContext> OnModuleInitialized(Action<PipelineModuleInitializedEventArgs> del)
        {
            return AddHandler((o, ea) => del(ea));
        }

        public IPipelineBuilder<TPipeline, TContext> OnPipeLineStageExectue(
            Action<object, PipelineEventFiringEventArgs> del)
        {
            return AddHandler((o, e) => del(o, e));
        }

        public IPipelineBuilder<TPipeline, TContext> OnPipeLineStageExectue(Action<PipelineEventFiringEventArgs> del)
        {
            return AddHandler((o, e) => del(e));
        }

        public IPipelineBuilder<TPipeline, TContext> OnPipeLineStageExectued(
            Action<object, PipelineEventFiredEventArgs> del)
        {
            return AddHandler((o, e) => del(o, e));
        }

        public IPipelineBuilder<TPipeline, TContext> OnPipeLineStageExectued(Action<PipelineEventFiredEventArgs> del)
        {
            return AddHandler((o, e) => del(e));
        }

        public IPipelineBuilder<TPipeline, TContext> OnPipelineError(Func<PipelineException, bool> del)
        {
            _onError = del;
            return this;
        }

        public IPipelineBuilder<TPipeline, TContext> RegisterModule<T>(T module) where T : IPipelineModule<TPipeline>
        {
            _modules.Add(module);
            return this;
        }

        public IPipelineBuilder<TPipeline, TContext> RegisterModule<T>() where T : IPipelineModule<TPipeline>, new()
        {
            _modules.Add(new T());
            return this;
        }

        public IPipeline<TContext> Make()
        {
            var pipelineConfig = new PipelineConfig(_pipelineSteps.PipelineName);

            // loop through dynamically loaded modules and create instances of each
            foreach (var module in pipelineConfig.Modules)
            {
                try
                {
                    var moduleType = Type.GetType(module.Type);

                    if (moduleType == null) continue;

                    if (_moduleInitalizing != null)
                    {
                        var args = new PipelineModuleInitializingEventArgs(_pipelineSteps.PipelineName, module);
                        _moduleInitalizing(this, args);
                        // if the event says don't load then move to next module
                        if (args.Cancel) continue;
                    }

                    var instance = Activator.CreateInstance(moduleType) as IPipelineModule<TPipeline>;

                    if (instance == null) continue;

                    instance.RegisterModuleWithPipeline(_pipelineSteps, module.Parameters);

                    if (_moduleInitalized == null) continue;
                    var loadedArgs = new PipelineModuleInitializedEventArgs(_pipelineSteps.PipelineName, module);
                    _moduleInitalized(this, loadedArgs);
                }
                catch (Exception)
                {
                    // swallow exceptions thrown when trying to load modules. 
                    // so if you try and register a module and it can't be created/breaks when initializing then the pipeline will still be functional,
                    // just missing the module(s) that failed to dynamically load. 
                }
            }

            // load any hardcoded modules
            // no try catch here as these are hard coded modules. 
            foreach (var module in _modules)
            {
                var modType = module.GetType();
                if (_moduleInitalizing != null)
                {
                    var loadingArgs = new PipelineModuleInitializingEventArgs(_pipelineSteps.PipelineName, modType.Name,
                        modType.AssemblyQualifiedName);
                    _moduleInitalizing(this, loadingArgs);
                    // if the event says don't load then move to next module
                    if (loadingArgs.Cancel) continue;
                }

                module.RegisterModuleWithPipeline(_pipelineSteps);

                if (_moduleInitalized == null) continue;
                var loadedArgs = new PipelineModuleInitializedEventArgs(_pipelineSteps.PipelineName, modType.Name,
                    modType.AssemblyQualifiedName);
                _moduleInitalized(this, loadedArgs);
            }

            var def = _pipelineSteps.ResolveActions();

            return new Pipeline<TContext>(def)
            {
                PipelineStageExecuted = _pipelineStageExecuted,
                PipelineStageExecuting = _pipelineStageExecuting,
                OnError = _onError
            };
        }

        private IPipelineBuilder<TPipeline, TContext> AddHandler(EventHandler<PipelineModuleInitializingEventArgs> del)
        {
            _moduleInitalizing += del;
            return this;
        }

        private IPipelineBuilder<TPipeline, TContext> AddHandler(EventHandler<PipelineModuleInitializedEventArgs> del)
        {
            _moduleInitalized += del;
            return this;
        }

        private IPipelineBuilder<TPipeline, TContext> AddHandler(EventHandler<PipelineEventFiredEventArgs> del)
        {
            _pipelineStageExecuted += del;
            return this;
        }

        private IPipelineBuilder<TPipeline, TContext> AddHandler(EventHandler<PipelineEventFiringEventArgs> del)
        {
            _pipelineStageExecuting += del;
            return this;
        }
    }

    public static class PipelineBuilder
    {
        public static IPipelineBuilder<TPipeline, TContext> CreatePipeline<TPipeline, TContext>()
            where TPipeline : PipelineSteps<TContext>, new()
            where TContext : PipelineContext
        {
            return new PipelineBuilder<TPipeline, TContext>();
        }
    }
}