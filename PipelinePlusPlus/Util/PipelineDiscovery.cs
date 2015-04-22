using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Transactions;
using PipelinePlusPlus.Attributes;
using PipelinePlusPlus.Core;

namespace PipelinePlusPlus.Util
{
    internal class PipelineDiscovery : IPipelineDiscovery
    {
        public PipelineDefinition<TContext> Discover<TContext>(PipelineSteps pipelineSteps)
            where TContext : PipelineContext
        {
            var properties =
                pipelineSteps.GetType()
                    .GetProperties()
                    .Where(p => p.PropertyType == typeof (PipelineStep<TContext>))
                    .ToList();

            if (!properties.Any())
                throw new PipelineDicoveryException(
                    string.Format("No properties found on the Pipeline Definition '{0}'. Discovery Aborted",
                        pipelineSteps.PipelineName));

            var sortedProperties = properties.Select(p => CreateDefinition<TContext>(p, pipelineSteps)).ToList();
            sortedProperties.Sort(new PipelineStepComparer<TContext>());

            var pipelineScopeOption = RequiredTransactionScope(sortedProperties);

            return new PipelineDefinition<TContext>(sortedProperties, pipelineScopeOption, pipelineSteps.PipelineName);
        }

        private static PipelineStepDefinintion<TContext> CreateDefinition<TContext>(PropertyInfo prop,
            PipelineSteps steps) where TContext : PipelineContext
        {
            //get the decorating attribute off. 
            var attr = prop.GetCustomAttributes(typeof (PipelineStepAttribute), true)
                .Cast<PipelineStepAttribute>()
                .FirstOrDefault();
            // has to be done. shame.
            if (attr == null)
                throw new PipelineStepException(
                    String.Format(
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
            IEnumerable<PipelineStepDefinintion<TContext>> defs) where TContext : PipelineContext
        {
            return defs.Any(d =>
                d.Attr.TransactionScopeOption == TransactionScopeOption.RequiresNew ||
                d.Attr.TransactionScopeOption == TransactionScopeOption.Required)
                ? TransactionScopeOption.Required
                : TransactionScopeOption.Suppress;
        }

        internal class PipelineStepComparer<TContext> : IComparer<PipelineStepDefinintion<TContext>>
            where TContext : PipelineContext
        {
            int IComparer<PipelineStepDefinintion<TContext>>.Compare(PipelineStepDefinintion<TContext> x,
                PipelineStepDefinintion<TContext> y)
            {
                return x.Attr.Order.CompareTo(y.Attr.Order);
            }
        }
    }
}