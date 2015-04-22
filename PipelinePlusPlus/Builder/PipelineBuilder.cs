using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Runtime.InteropServices;
using PipelinePlusPlus.Core;
using PipelinePlusPlus.Definition;
using PipelinePlusPlus.EventArgs;
using PipelinePlusPlus.Util;

namespace PipelinePlusPlus.Builder
{
    public class PipelineBuilder<TPipeline, TContext> : IPipelineBuilder<TPipeline, TContext>
        where TPipeline : PipelineSteps, new()
        where TContext : PipelineContext
    {
        private readonly IPipelineModuleManager _moduleManager;
        private readonly IList<PipelineModule<TPipeline, TContext>> _modules = new List<PipelineModule<TPipeline, TContext>>();
        private EventHandler<PipelineModuleInitializedEventArgs> _moduleInitialized;
        private EventHandler<PipelineModuleInitializingEventArgs> _moduleInitializing;
        private Func<PipelineException, bool> _onError = e => true;
        private EventHandler<PipelineEventFiredEventArgs> _pipelineStageExecuted;
        private EventHandler<PipelineEventFiringEventArgs> _pipelineStageExecuting;


        internal PipelineBuilder(IPipelineModuleManager moduleManager)
        {
            _moduleManager = moduleManager;

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

        public IPipelineBuilder<TPipeline, TContext> RegisterModule<T>(T module) where T : PipelineModule<TPipeline,TContext>
        {
            _modules.Add(module);
            return this;
        }

        public IPipelineBuilder<TPipeline, TContext> RegisterModule<T>() where T : PipelineModule<TPipeline, TContext>, new()
        {
            _modules.Add(new T());
            return this;
        }

        public IPipeline<TContext> Make(Func<System.Configuration.Configuration> getConfig)
        {
            var pipelineSteps = new TPipeline();
            // get config from xml
            var pipelineConfig = new PipelineConfig<TContext>(pipelineSteps.PipelineName, getConfig);

            //register modules
            _moduleManager.RegisterDynamicModules(pipelineSteps, pipelineConfig, _moduleInitializing, _moduleInitialized);
            _moduleManager.RegisterModules(pipelineSteps, _modules, _moduleInitializing, _moduleInitialized);
            //resolve steps
            var def = pipelineSteps.ResolveActions<TContext>();

            // create execution context
            var executionContext = new PipelineExecutionContext<TContext>(def, _onError)
            {
                PipelineStageExecuted = _pipelineStageExecuted,
                PipelineStageExecuting = _pipelineStageExecuting
            };

            return new Pipeline<TContext>(executionContext);
        }

        private IPipelineBuilder<TPipeline, TContext> AddHandler(EventHandler<PipelineModuleInitializingEventArgs> del)
        {
            _moduleInitializing += del;
            return this;
        }

        private IPipelineBuilder<TPipeline, TContext> AddHandler(EventHandler<PipelineModuleInitializedEventArgs> del)
        {
            _moduleInitialized += del;
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
            where TPipeline : PipelineSteps, new()
            where TContext : PipelineContext
        {
            return new PipelineBuilder<TPipeline, TContext>(new PipelineModuleMananger());
        }
    }
}