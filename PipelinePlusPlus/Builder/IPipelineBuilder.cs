using System;
using System.Collections;
using System.Collections.Generic;
using Pipeline;
using Pipeline.Definition;
using Pipeline.EventArgs;
using Pipeline.Support_Code;

namespace PipelinePlusPlus.Builder
{
    // the pipeline definition, 
    // this is where events are registered
    // a implementor of this platform will define this and the event order. 
    public abstract class Pipeline<TContext>
    {

        protected TContext Context { get; private set; }

        protected Pipeline()
        {
        }

        internal void Registercontext(TContext context)
        {
            Context = context;
        }
    }

    internal sealed class PipelineException : Exception
    {
        public PipelineException(string reason, string moduleName)
            : base(string.Format("Pipeline cancelled by module: {0}. Module '{1}'.", reason, moduleName))
        {

        }
        public PipelineException(string moduleName, Exception innerException)
            : base(string.Format("ERROR when executing pipeline module '{0}'. pipeline will terminate.", moduleName), innerException)
        {
        }
    }

    public abstract class PipelineContext
    {
        private PipelineException _exception;

        public void Cancel(string reason, Module executingModule)
        {
            _exception = new PipelineException(reason, executingModule.Type);
        }

        public void RegisterPipelineError(Exception e, Module executingModule)
        {
            _exception = new PipelineException(executingModule.Type, e);
        }

        public bool CancelExecution { get { return _exception == null; } }
    }

    public interface IPipelineBuilder<TEvents, TContext>
        where TEvents : PipelineEvents, new()
        where TContext : PipelineContext
    {
        IPipelineBuilder<TEvents, TContext> OnModuleInitialize(Action<object, PipelineModuleInitializingEventArgs> del);
        IPipelineBuilder<TEvents, TContext> OnModuleInitialized(Action<object, PipelineModuleInitializedEventArgs> del);
        IPipelineBuilder<TEvents, TContext> OnModuleInitialize(Action<PipelineModuleInitializingEventArgs> del);
        IPipelineBuilder<TEvents, TContext> OnModuleInitialized(Action<PipelineModuleInitializedEventArgs> del);

        IPipelineBuilder<TEvents, TContext> OnPipeLineStageExectue(Action<object, PipelineEventFiringEventArgs> del);
        IPipelineBuilder<TEvents, TContext> OnPipeLineStageExectue(Action<PipelineEventFiringEventArgs> del);
        IPipelineBuilder<TEvents, TContext> OnPipeLineStageExectued(Action<object, PipelineEventFiredEventArgs> del);
        IPipelineBuilder<TEvents, TContext> OnPipeLineStageExectued(Action<PipelineEventFiredEventArgs> del);

    }

    public delegate void PipelineAction<in T>(T context) where T : PipelineContext;

    public interface IPipeline<in TEvents, TContext>
        where TEvents : PipelineEvents, new()
        where TContext : PipelineContext
    {
        void Execute(PipelineAction<TContext> act, TContext context);
        void Execute(PipelineAction<TContext> pipelineEvent, TContext context, TransactionScopeOption transactionScope);
        void Execute(TEvents pipelineEvents, TContext context);
    }

    public static class PipelineBuilder
    {
        // the name is the name of the pipeline in your config section. 
        // you can add modules to this section to dynamically load functionality into your pipeline. 
        public static IPipelineBuilder<TEvents, TContext> Create<TEvents, TContext>(string name)
            where TEvents : PipelineEvents, new()
            where TContext : PipelineContext
        {
            return new PipelineBuilder<TEvents, TContext>(name);
        }
    }

    public class PipelineBuilder<TEvents, TContext> : IPipelineBuilder<TEvents, TContext>
        where TEvents : PipelineEvents, new()
        where TContext : PipelineContext
    {
        private readonly string _pipelineName;
        private EventHandler<PipelineModuleInitializingEventArgs> _moduleInitalizing;
        private EventHandler<PipelineModuleInitializedEventArgs> _moduleInitalized;
        private EventHandler<PipelineEventFiredEventArgs> _pipelineStageExecuted;
        private EventHandler<PipelineEventFiringEventArgs> _pipelineStageExecuting;


        internal PipelineBuilder(string name)
        {
            _pipelineName = name;
        }



        // internal methods do the work. 
        // more specific methods just pass to these. 
        private IPipelineBuilder<TEvents, TContext> AddHandler(EventHandler<PipelineModuleInitializingEventArgs> del)
        {
            _moduleInitalizing += del;
            return this;
        }

        private IPipelineBuilder<TEvents, TContext> AddHandler(EventHandler<PipelineModuleInitializedEventArgs> del)
        {
            _moduleInitalized += del;
            return this;
        }

        private IPipelineBuilder<TEvents, TContext> AddHandler(EventHandler<PipelineEventFiredEventArgs> del)
        {
            _pipelineStageExecuted += del;
            return this;
        }

        private IPipelineBuilder<TEvents, TContext> AddHandler(EventHandler<PipelineEventFiringEventArgs> del)
        {
            _pipelineStageExecuting += del;
            return this;
        }




        public IPipelineBuilder<TEvents, TContext> OnModuleInitialize(Action<object, PipelineModuleInitializingEventArgs> del)
        {
            return AddHandler((o, ea) => del(o, ea));
        }

        public IPipelineBuilder<TEvents, TContext> OnModuleInitialized(Action<object, PipelineModuleInitializedEventArgs> del)
        {
            return AddHandler((o, ea) => del(o, ea));
        }

        public IPipelineBuilder<TEvents, TContext> OnModuleInitialize(Action<PipelineModuleInitializingEventArgs> del)
        {
            return AddHandler((o, ea) => del(ea));
        }

        public IPipelineBuilder<TEvents, TContext> OnModuleInitialized(Action<PipelineModuleInitializedEventArgs> del)
        {
            return AddHandler((o, ea) => del(ea));
        }

        public IPipelineBuilder<TEvents, TContext> OnPipeLineStageExectue(Action<object, PipelineEventFiringEventArgs> del)
        {
            return AddHandler((o, e) => del(o, e));
        }

        public IPipelineBuilder<TEvents, TContext> OnPipeLineStageExectue(Action<PipelineEventFiringEventArgs> del)
        {
            return AddHandler((o, e) => del(e));
        }

        public IPipelineBuilder<TEvents, TContext> OnPipeLineStageExectued(Action<object, PipelineEventFiredEventArgs> del)
        {
            return AddHandler((o, e) => del(o, e));
        }

        public IPipelineBuilder<TEvents, TContext> OnPipeLineStageExectued(Action<PipelineEventFiredEventArgs> del)
        {
            return AddHandler((o, e) => del(e));
        }
    }

    internal class thing
    {

        internal void Do(Action<object, PipelineModuleInitializingEventArgs> act)
        {

            //var b = PipelineBuilder.Create<string, string>("test").OnModuleInitialize(args =>
            //{

            //});
        }
    }
}
