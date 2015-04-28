using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Transactions;
using MoreLinq;
using PipelinePlusPlus.Core.Attributes;
using PipelinePlusPlus.Core.Context;
using PipelinePlusPlus.Core.DynamicConfig;
using PipelinePlusPlus.Core.EventArgs;
using PipelinePlusPlus.Core.Exceptions;
using PipelinePlusPlus.Core.Modules;
using PipelinePlusPlus.Core.Modules.Mananger;
using PipelinePlusPlus.Core.Steps;

namespace PipelinePlusPlus.Core.Discovery
{
    internal class PipelineDiscovery<TPipeline, TContext> : IPipelineDiscovery<TPipeline, TContext> where TContext : PipelineStepContext where TPipeline : PipelineSteps
    {
        private readonly IDynamicModuleConfig _dynamicModuleConfig;
        private readonly EventHandler<PipelineModuleInitializedEventArgs> _moduleInitializedHandler;
        private readonly EventHandler<PipelineModuleInitializingEventArgs> _moduleInitializingHandler;
        private readonly IPipelineModuleManager _moduleManager;
        private readonly TPipeline _pipelineSteps;

        public PipelineDiscovery(IDynamicModuleConfig dynamicModuleConfig, IPipelineModuleManager moduleManager, TPipeline pipelineSteps, EventHandler<PipelineModuleInitializedEventArgs> moduleInitializedHandler, EventHandler<PipelineModuleInitializingEventArgs> moduleInitializingHandler)
        {
            _dynamicModuleConfig = dynamicModuleConfig;
            _pipelineSteps = pipelineSteps;
            _moduleManager = moduleManager;
            _moduleInitializedHandler = moduleInitializedHandler;
            _moduleInitializingHandler = moduleInitializingHandler;
        }

        public IPipelineDefinition<TContext> ResolvePipeline(IEnumerable<PipelineModule<TPipeline, TContext>> modules, Configuration appConfig)
        {
            // initialise the pipeline steps and grab definitions for method invocation and attribute info.
            var definition = CreateDefinition();

            // load config from app config
            var dynamicConfig = _dynamicModuleConfig.GetConfig(_pipelineSteps.PipelineName, appConfig);

            // register modules aganst the pipeline
            _moduleManager.RegisterDynamicModules<TPipeline, TContext>(_pipelineSteps, dynamicConfig, _moduleInitializingHandler, _moduleInitializedHandler);
            _moduleManager.RegisterModules(_pipelineSteps, modules, _moduleInitializingHandler, _moduleInitializedHandler);

            return definition;
        }

        private IPipelineDefinition<TContext> CreateDefinition()
        {
            var properties = _pipelineSteps.GetType()
                .GetProperties()
                .Where(p => p.PropertyType == typeof (PipelineStep<TContext>))
                .ToList();

            if (!properties.Any())
            {
                throw new PipelineDicoveryException(string.Format("No properties found on the Pipeline Definition '{0}'. Discovery Aborted", _pipelineSteps.PipelineName));
            }
            var propertyDefinitions = properties.Select(p => CreateStepDefinition(p, _pipelineSteps))
                .ToList();

            // check to see if the list contains unique order values, the don't have to be in sequence, they just can't be duplicated in the sequence 
            if (propertyDefinitions.DistinctBy(p => p.Attr.Order)
                .Count() != propertyDefinitions.Count)
            {
                throw new PipelineDicoveryException("The order value in the PipelineStepAttribute has duplicates. Please review your steps and ensure that the SequenceOrder value is set correctly and not duplicated per task.");
            }

            var sortedProperties = propertyDefinitions.OrderBy(p => p, new PipelineStepComparer())
                .ToList();

            var pipelineScopeOption = RequiredTransactionScope(sortedProperties);

            return new PipelineDefinition<TContext>(sortedProperties, pipelineScopeOption, _pipelineSteps.PipelineName);
        }

        private static PipelineStepDefinintion<TContext> CreateStepDefinition(PropertyInfo prop, TPipeline steps)
        {
            //get the decorating attribute off. 
            var attr = prop.GetCustomAttributes(typeof (PipelineStepAttribute), true)
                .Cast<PipelineStepAttribute>()
                .FirstOrDefault();
            // has to be done. shame.
            if (attr == null)
            {
                throw new PipelineDicoveryException(string.Format("Step '{0}' has no PipelineStepAttribute defined. Please review the pipeline definition '{1}'", prop.Name, steps.PipelineName));
            }

            // get current value
            var step = prop.GetValue(steps, null) as PipelineStep<TContext>;
            // if not initialised
            if (step == null)
            {
                // create
                step = new PipelineStep<TContext>();
                //set property on steps instance
                prop.SetValue(steps, step);
            }
            // return the definition
            return new PipelineStepDefinintion<TContext>(prop.Name, attr, step);
        }

        private static TransactionScopeOption RequiredTransactionScope(ICollection<PipelineStepDefinintion<TContext>> defs)
        {
            if (defs.Any(d => d.Attr.TransactionScopeOption == TransactionScopeOption.RequiresNew))
            {
                return TransactionScopeOption.RequiresNew;
            }
            return defs.Any(d => d.Attr.TransactionScopeOption == TransactionScopeOption.Required) ? TransactionScopeOption.Required : TransactionScopeOption.Suppress;
        }

        internal class PipelineStepComparer : IComparer<PipelineStepDefinintion<TContext>>
        {
            int IComparer<PipelineStepDefinintion<TContext>>.Compare(PipelineStepDefinintion<TContext> x, PipelineStepDefinintion<TContext> y)
            {
                return x.Attr.Order.CompareTo(y.Attr.Order);
            }
        }
    }
}