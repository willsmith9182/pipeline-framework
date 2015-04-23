using System;
using System.Collections.Generic;
using System.Configuration;
using PipelinePlusPlus.Core;
using PipelinePlusPlus.Core.Context;
using PipelinePlusPlus.Core.Discovery;
using PipelinePlusPlus.Core.DynamicConfig;
using PipelinePlusPlus.Core.EventArgs;
using PipelinePlusPlus.Core.Exceptions;
using PipelinePlusPlus.Core.Modules;
using PipelinePlusPlus.Core.Modules.Mananger;
using PipelinePlusPlus.Core.Steps;

namespace PipelinePlusPlus.Builder
{
    public static class PipelineBuilder
    {
        public static IPipelineBuilder<TPipeline, TContext> CreatePipeline<TPipeline, TContext>()
            where TPipeline : PipelineSteps, new()
            where TContext : PipelineStepContext { return new PipelineBuilder<TPipeline, TContext>(new PipelineModuleMananger(), new PipelineDiscovery(), new DynamicModuleConfig()); }
    }

    public class PipelineBuilder<TPipeline, TContext> : IPipelineBuilder<TPipeline, TContext>
        where TPipeline : PipelineSteps, new()
        where TContext : PipelineStepContext
    {
        #region depencencies
        private readonly IPipelineModuleManager _moduleManager;
        private readonly IDynamicModuleConfig _dynamicModuleConfig;
        private readonly IPipelineDiscovery _pipelineDiscovery;
        #endregion

        // builder state
        private readonly IList<PipelineModule<TPipeline, TContext>> _modules;


        internal PipelineBuilder(IPipelineModuleManager moduleManager, IPipelineDiscovery pipelineDiscovery, IDynamicModuleConfig dynamicModuleConfig)
        {
            // dependecies
            _moduleManager = moduleManager;
            _pipelineDiscovery = pipelineDiscovery;
            _dynamicModuleConfig = dynamicModuleConfig;

            // initialise handlers to null.
            ModuleInitializedHandler = null;
            ModuleInitializingHandler = null;
            PipelineStageExecutedHandler = null;
            PipelineStageExecutingHandler = null;
            // stop pipeline on error
            OnError = e => true;
            // initialise module list
            _modules = new List<PipelineModule<TPipeline, TContext>>();
        }

        public EventHandler<PipelineModuleInitializedEventArgs> ModuleInitializedHandler { get; private set; }
        public EventHandler<PipelineModuleInitializingEventArgs> ModuleInitializingHandler { get; private set; }
        public Func<PipelineException, bool> OnError { get; private set; }
        public EventHandler<PipelineEventFiredEventArgs> PipelineStageExecutedHandler { get; private set; }
        public EventHandler<PipelineEventFiringEventArgs> PipelineStageExecutingHandler { get; private set; }

        // pass through methods. 
        public IPipelineBuilder<TPipeline, TContext> OnModuleInitialize(Action<object, PipelineModuleInitializingEventArgs> del) { return AddHandler((o, ea) => del(o, ea)); }
        public IPipelineBuilder<TPipeline, TContext> OnModuleInitialized(Action<object, PipelineModuleInitializedEventArgs> del) { return AddHandler((o, ea) => del(o, ea)); }
        public IPipelineBuilder<TPipeline, TContext> OnModuleInitialize(Action<PipelineModuleInitializingEventArgs> del) { return AddHandler((o, ea) => del(ea)); }
        public IPipelineBuilder<TPipeline, TContext> OnModuleInitialized(Action<PipelineModuleInitializedEventArgs> del) { return AddHandler((o, ea) => del(ea)); }
        public IPipelineBuilder<TPipeline, TContext> OnPipeLineStageExectue(Action<object, PipelineEventFiringEventArgs> del) { return AddHandler((o, e) => del(o, e)); }
        public IPipelineBuilder<TPipeline, TContext> OnPipeLineStageExectue(Action<PipelineEventFiringEventArgs> del) { return AddHandler((o, e) => del(e)); }
        public IPipelineBuilder<TPipeline, TContext> OnPipeLineStageExectued(Action<object, PipelineEventFiredEventArgs> del) { return AddHandler((o, e) => del(o, e)); }
        public IPipelineBuilder<TPipeline, TContext> OnPipeLineStageExectued(Action<PipelineEventFiredEventArgs> del) { return AddHandler((o, e) => del(e)); }

        public IPipelineBuilder<TPipeline, TContext> OnPipelineError(Func<PipelineException, bool> del)
        {
            OnError = del;
            return this;
        }

        public IPipelineBuilder<TPipeline, TContext> RegisterModule<T>(T module) where T : PipelineModule<TPipeline, TContext>
        {
            _modules.Add(module);
            return this;
        }

        public IPipelineBuilder<TPipeline, TContext> RegisterModule<T>() where T : PipelineModule<TPipeline, TContext>, new()
        {
            _modules.Add(new T());
            return this;
        }

        private IPipelineBuilder<TPipeline, TContext> AddHandler(EventHandler<PipelineModuleInitializingEventArgs> del)
        {
            ModuleInitializingHandler += del;
            return this;
        }

        private IPipelineBuilder<TPipeline, TContext> AddHandler(EventHandler<PipelineModuleInitializedEventArgs> del)
        {
            ModuleInitializedHandler += del;
            return this;
        }

        private IPipelineBuilder<TPipeline, TContext> AddHandler(EventHandler<PipelineEventFiredEventArgs> del)
        {
            PipelineStageExecutedHandler += del;
            return this;
        }

        private IPipelineBuilder<TPipeline, TContext> AddHandler(EventHandler<PipelineEventFiringEventArgs> del)
        {
            PipelineStageExecutingHandler += del;
            return this;
        }

        public IPipeline<TContext> Make(Configuration appConfig)
        {
            var pipelineSteps = new TPipeline();
            // discover pipeline, does step check and inistalises pipeline ready for execution
            var pipelineDefinition = _pipelineDiscovery.Discover<TContext>(pipelineSteps);

            // get dynamic config information
            var pipelineConfig = _dynamicModuleConfig.GetConfig(pipelineSteps.PipelineName, appConfig);

            //register modules discovered or registered
            _moduleManager.RegisterDynamicModules<TPipeline, TContext>(pipelineSteps, pipelineConfig, ModuleInitializingHandler, ModuleInitializedHandler);
            _moduleManager.RegisterModules(pipelineSteps, _modules, ModuleInitializingHandler, ModuleInitializedHandler);

            // create execution context
            var executionContext = new PipelineExecutionContext<TContext>(pipelineDefinition, OnError) { PipelineStageExecuted = PipelineStageExecutedHandler, PipelineStageExecuting = PipelineStageExecutingHandler };

            return new Pipeline<TContext>(executionContext);
        }
    }
}