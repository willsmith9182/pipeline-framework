using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Transactions;
using Pipeline.Attributes;
using Pipeline.EventArgs;
using Pipeline.Support_Code;
using TransactionScopeOption = Pipeline.Support_Code.TransactionScopeOption;

namespace Pipeline
{
    public class Backbone<TEvents, TContext> : Backbone<TEvents>, IBackbone<TEvents, TContext>
        where TEvents : PipelineEvents, new()
        where TContext : PipelineContext
    {
        public Backbone(string pipelineName)
            : base(pipelineName)
        {
        }

        public Backbone(Definition.Pipeline pipeline)
            : base(pipeline)
        {
        }

        public event EventHandler<PipelineEventFiringEventArgs> PipelineEventFiring;
        public event EventHandler<PipelineEventFiredEventArgs> PipelineEventFired;

        public void Execute(PipelineContext<TContext> pipelineEvent, TContext context)
        {
            Execute(pipelineEvent, context, TransactionScopeOption.Suppress);
        }

        public void Execute(PipelineContext<TContext> pipelineEvent, TContext context,
            TransactionScopeOption transactionScope)
        {
            Contract.Requires(pipelineEvent != null);
            Contract.Requires(context != null);

            var pipeline = GetPipelineDefinition();

            var scopeOption =
                GetTransactionScopeOption(transactionScope);

            if (pipelineEvent != null)
            {
                using (var eventScope = new TransactionScope(scopeOption))
                {
                    var args =
                        new PipelineEventFiringEventArgs(pipeline.Name);
                    OnPipelineEventFiring(args);

                    if (!args.Cancel)
                    {
                        if (pipeline.InvokeAll)
                        {
                            pipelineEvent(context);
                        }
                        else
                        {
                            var list = pipelineEvent.GetInvocationList();

                            foreach (var item in list.OfType<PipelineContext<TContext>>())
                            {
                                item(context);
                                if (context.Cancel)
                                    break;
                            }
                        }

                        OnPipelineEventFired(new PipelineEventFiredEventArgs(pipeline.Name));
                    }

                    eventScope.Complete();
                }
            }
        }

        public void Execute(TEvents pipelineEvents, TContext context)
        {
            Contract.Requires(pipelineEvents != null);
            Contract.Requires(context != null);

            var pipeline = GetPipelineDefinition();

            var properties = pipelineEvents.GetType().GetProperties();

            var sortedProperties = properties.ToList();
            sortedProperties.Sort(new PropertyComparer());

            var pipelineScopeOption = TransactionRequirement(sortedProperties);

            using (var pipelineScope = new TransactionScope(pipelineScopeOption))
            {
                sortedProperties.ForEach(property =>
                {
                    var attributes =
                        property.GetCustomAttributes(typeof (PipelineEventAttribute), true);

                    if (attributes.Length <= 0) return;

                    var attr = (PipelineEventAttribute) attributes[0];

                    var scopeOption =
                        GetTransactionScopeOption(attr.TransactionScopeOption);

                    var value = property.GetValue(pipelineEvents, null);
                    var eventProp = (PipelineContext<TContext>) value;

                    if (eventProp == null) return;

                    using (var eventScope = new TransactionScope(scopeOption))
                    {
                        var args = new PipelineEventFiringEventArgs(pipeline.Name, property.Name);
                        OnPipelineEventFiring(args);

                        if (!args.Cancel)
                        {
                            if (pipeline.InvokeAll)
                            {
                                eventProp(context);
                            }
                            else
                            {
                                var list = eventProp.GetInvocationList();

                                foreach (var item in list.OfType<PipelineContext<TContext>>())
                                {
                                    item(context);
                                    if (context.Cancel)
                                        break;
                                }
                            }

                            OnPipelineEventFired(new PipelineEventFiredEventArgs(pipeline.Name, property.Name));
                        }

                        eventScope.Complete();
                    }
                });

                pipelineScope.Complete();
            }
        }

        public static void ExecutePipelineEvent(string pipelineName, PipelineContext<TContext> pipelineEvent,
            TContext context)
        {
            new Backbone<TEvents, TContext>(pipelineName).Execute(pipelineEvent, context,
                TransactionScopeOption.Suppress);
        }

        public static void ExecutePipelineEvent(string pipelineName, PipelineContext<TContext> pipelineEvent,
            TContext context, TransactionScopeOption transactionScope)
        {
            new Backbone<TEvents, TContext>(pipelineName).Execute(pipelineEvent, context, transactionScope);
        }

        public static void ExecutePipeline(string pipelineName, TEvents pipelineEvents, TContext context)
        {
            new Backbone<TEvents, TContext>(pipelineName).Execute(pipelineEvents, context);
        }

        public static void ExecutePipelineEvent(Definition.Pipeline pipeline, PipelineContext<TContext> pipelineEvent,
            TContext context)
        {
            new Backbone<TEvents, TContext>(pipeline).Execute(pipelineEvent, context, TransactionScopeOption.Suppress);
        }

        public static void ExecutePipelineEvent(Definition.Pipeline pipeline, PipelineContext<TContext> pipelineEvent,
            TContext context, TransactionScopeOption transactionScope)
        {
            new Backbone<TEvents, TContext>(pipeline).Execute(pipelineEvent, context, transactionScope);
        }

        public static void ExecutePipeline(Definition.Pipeline pipeline, TEvents pipelineEvents, TContext context)
        {
            new Backbone<TEvents, TContext>(pipeline).Execute(pipelineEvents, context);
        }

        protected virtual void OnPipelineEventFiring(PipelineEventFiringEventArgs e)
        {
            if (PipelineEventFiring != null)
                PipelineEventFiring(this, e);
        }

        protected virtual void OnPipelineEventFired(PipelineEventFiredEventArgs e)
        {
            if (PipelineEventFired != null)
                PipelineEventFired(this, e);
        }

        private System.Transactions.TransactionScopeOption GetTransactionScopeOption(
            TransactionScopeOption transactionScopeOption)
        {
            switch (transactionScopeOption)
            {
                case TransactionScopeOption.RequiredNew:
                    return System.Transactions.TransactionScopeOption.RequiresNew;
                case TransactionScopeOption.Suppress:
                    return System.Transactions.TransactionScopeOption.Suppress;
                default:
                    return System.Transactions.TransactionScopeOption.Required;
            }
        }

        private static System.Transactions.TransactionScopeOption TransactionRequirement(
            IEnumerable<PropertyInfo> sortedProperties)
        {
            var pipelineScopeOption = System.Transactions.TransactionScopeOption.Suppress;

            // ReSharper disable once LoopCanBeConvertedToQuery
            // makes it harder to understand if done as linq body.
            foreach (var property in sortedProperties)
            {
                var attributes = property.GetCustomAttributes(typeof (PipelineEventAttribute), true);
                if (attributes.Length <= 0) continue;
                var attr = (PipelineEventAttribute) attributes[0];
                if (attr.TransactionScopeOption == TransactionScopeOption.Suppress) continue;
                pipelineScopeOption = System.Transactions.TransactionScopeOption.Required;
                break;
            }

            return pipelineScopeOption;
        }
    }
}