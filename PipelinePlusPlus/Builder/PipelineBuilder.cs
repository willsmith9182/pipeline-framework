using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using PipelinePlusPlus.Core;
using PipelinePlusPlus.Core.Context;
using PipelinePlusPlus.Core.Discovery;
using PipelinePlusPlus.Core.EventArgs;
using PipelinePlusPlus.Core.Exceptions;
using PipelinePlusPlus.Core.Modules;
using PipelinePlusPlus.Core.Steps;

namespace PipelinePlusPlus.Builder
{
    public static class PipelineBuilder
    {
        public static IPipelineBuilder<TPipeline, TContext> CreatePipeline<TPipeline, TContext>()
            where TPipeline : PipelineSteps, new()
            where TContext : PipelineStepContext
        {
            return new PipelineBuilder<TPipeline, TContext>();
        }
    }

    public class PipelineBuilder<TPipeline, TContext> : IPipelineBuilder<TPipeline, TContext>
        where TPipeline : PipelineSteps, new()
        where TContext : PipelineStepContext
    {
        private readonly IDiscoveryFactory _discoveryFactory;
        // builder state
        private readonly IList<PipelineModule<TPipeline, TContext>> _modules;
        // ncrunch: no coverage start
        internal PipelineBuilder()
            : this(new DiscoveryFactory())
        {
        }

        // ncrunch: no coverage end

        internal PipelineBuilder(IDiscoveryFactory discoveryFactory)
        {
            _discoveryFactory = discoveryFactory;

            // initialise handlers to null.
            ModuleInitializedHandler = (e, a) => { };
            ModuleInitializingHandler = (e, a) => { };
            PipelineStageExecutedHandler = (e, a) => { };
            PipelineStageExecutingHandler = (e, a) => { };
            // stop pipeline on error
            OnError = e => true;
            // initialise module list
            _modules = new List<PipelineModule<TPipeline, TContext>>();
        }

        internal IReadOnlyCollection<PipelineModule<TPipeline, TContext>> Modules
        {
            get { return new ReadOnlyCollection<PipelineModule<TPipeline, TContext>>(_modules); }
        }

        internal EventHandler<PipelineModuleInitializedEventArgs> ModuleInitializedHandler { get; private set; }
        internal EventHandler<PipelineModuleInitializingEventArgs> ModuleInitializingHandler { get; private set; }
        internal Func<PipelineException, bool> OnError { get; private set; }
        internal EventHandler<PipelineEventFiredEventArgs> PipelineStageExecutedHandler { get; private set; }
        internal EventHandler<PipelineEventFiringEventArgs> PipelineStageExecutingHandler { get; private set; }
        // pass through methods. 
        public IPipelineBuilder<TPipeline, TContext> OnModuleInitialize(Action<object, PipelineModuleInitializingEventArgs> del)
        {
            return AddHandler((o, ea) => del(o, ea));
        }

        public IPipelineBuilder<TPipeline, TContext> OnModuleInitialized(Action<object, PipelineModuleInitializedEventArgs> del)
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

        public IPipelineBuilder<TPipeline, TContext> OnPipeLineStageExectue(Action<object, PipelineEventFiringEventArgs> del)
        {
            return AddHandler((o, e) => del(o, e));
        }

        public IPipelineBuilder<TPipeline, TContext> OnPipeLineStageExectue(Action<PipelineEventFiringEventArgs> del)
        {
            return AddHandler((o, e) => del(e));
        }

        public IPipelineBuilder<TPipeline, TContext> OnPipeLineStageExectued(Action<object, PipelineEventFiredEventArgs> del)
        {
            return AddHandler((o, e) => del(o, e));
        }

        public IPipelineBuilder<TPipeline, TContext> OnPipeLineStageExectued(Action<PipelineEventFiredEventArgs> del)
        {
            return AddHandler((o, e) => del(e));
        }

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

        public IPipeline<TContext> Make(Configuration appConfig)
        {
            var pipelineSteps = new TPipeline();

            var discovery = _discoveryFactory.GetDiscovery<TPipeline, TContext>(pipelineSteps, ModuleInitializedHandler, ModuleInitializingHandler);

            var pipelineDefinition = discovery.ResolvePipeline(_modules, appConfig);

            // create execution context
            var executionContext = new PipelineExecutionContext<TContext>(pipelineDefinition, OnError)
            {
                PipelineStageExecuted = PipelineStageExecutedHandler,
                PipelineStageExecuting = PipelineStageExecutingHandler
            };

            return new Pipeline<TContext>(executionContext);
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
    }
}