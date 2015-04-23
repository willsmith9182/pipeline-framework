using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Transactions;
using MoreLinq;
using PipelinePlusPlus.Core.Attributes;
using PipelinePlusPlus.Core.Context;
using PipelinePlusPlus.Core.Exceptions;
using PipelinePlusPlus.Core.Steps;

namespace PipelinePlusPlus.Core.Discovery
{
    internal class PipelineDiscovery : IPipelineDiscovery
    {
        public PipelineDefinition<TContext> Discover<TContext>(PipelineSteps pipelineSteps)
            where TContext : PipelineStepContext
        {
            var properties =
                pipelineSteps.GetType()
                    .GetProperties()
                    .Where(p => p.PropertyType == typeof(PipelineStep<TContext>))
                    .ToList();

            if (!properties.Any())
                throw new PipelineDicoveryException(
                    string.Format("No properties found on the Pipeline Definition '{0}'. Discovery Aborted",
                        pipelineSteps.PipelineName));
            var propertyDefinitions = properties.Select(p => CreateDefinition<TContext>(p, pipelineSteps)).ToList();

            // check to see if the list contains unique order values, the don't have to be in sequence, they just can't be duplicated in the sequence 
            if (propertyDefinitions.DistinctBy(p => p.Attr.Order).Count() != propertyDefinitions.Count)
                throw new PipelineDicoveryException("The order value in the PipelineStepAttribute has duplicates. Please review your steps and ensure that the SequenceOrder value is set correctly and not duplicated per task.");

            var sortedProperties = propertyDefinitions.OrderBy(p => p, new PipelineStepComparer<TContext>()).ToList();

            var pipelineScopeOption = RequiredTransactionScope(sortedProperties);

            return new PipelineDefinition<TContext>(sortedProperties, pipelineScopeOption, pipelineSteps.PipelineName);
        }

        private static PipelineStepDefinintion<TContext> CreateDefinition<TContext>(PropertyInfo prop,
            PipelineSteps steps) where TContext : PipelineStepContext
        {
            //get the decorating attribute off. 
            var attr = prop.GetCustomAttributes(typeof(PipelineStepAttribute), true)
                .Cast<PipelineStepAttribute>()
                .FirstOrDefault();
            // has to be done. shame.
            if (attr == null)
                throw new PipelineDicoveryException(
                    string.Format(
                        "Step '{0}' has no PipelineStepAttribute defined. Please review the pipeline definition '{1}'",
                        prop.Name, steps.PipelineName));

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

        private static TransactionScopeOption RequiredTransactionScope<TContext>(
            ICollection<PipelineStepDefinintion<TContext>> defs) where TContext : PipelineStepContext
        {
            if (defs.Any(d => d.Attr.TransactionScopeOption == TransactionScopeOption.RequiresNew)) return TransactionScopeOption.RequiresNew;
            return defs.Any(d => d.Attr.TransactionScopeOption == TransactionScopeOption.Required) ? TransactionScopeOption.Required : TransactionScopeOption.Suppress;
        }

        internal class PipelineStepComparer<TContext> : IComparer<PipelineStepDefinintion<TContext>>
            where TContext : PipelineStepContext
        {
            int IComparer<PipelineStepDefinintion<TContext>>.Compare(PipelineStepDefinintion<TContext> x,
                PipelineStepDefinintion<TContext> y)
            {
                return x.Attr.Order.CompareTo(y.Attr.Order);
            }
        }
    }
}