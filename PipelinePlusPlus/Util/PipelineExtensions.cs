using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Transactions;
using PipelinePlusPlus.Attributes;
using PipelinePlusPlus.Core;

namespace PipelinePlusPlus.Util
{
    internal static class PipelineExtensions
    {
        private static TransactionScopeOption RequiredTransactionScope<TContext>(
            this IEnumerable<PipelineStepDefinintion<TContext>> defs) where TContext : PipelineContext
        {
            return defs.Any(d =>
                d.Attr.TransactionScopeOption == TransactionScopeOption.RequiresNew ||
                d.Attr.TransactionScopeOption == TransactionScopeOption.Required)
                ? TransactionScopeOption.Required
                : TransactionScopeOption.Suppress;
        }

        private static PipelineStepDefinintion<TContext> CreateDefinition<TContext>(this PropertyInfo prop,
            PipelineSteps<TContext> steps) where TContext : PipelineContext
        {
            var attr = prop.GetCustomAttributes(typeof (PipelineStepAttribute), true)
                .Cast<PipelineStepAttribute>()
                .FirstOrDefault();
            var instance = (PipelineAction<TContext>) prop.GetValue(steps, null);

            return new PipelineStepDefinintion<TContext>(prop, attr, instance);
        }

        public static PipelineDefninition<TContext> ResolveActions<TContext>(this PipelineSteps<TContext> pipelineSteps)
            where TContext : PipelineContext
        {
            var properties = pipelineSteps.GetType().GetProperties().Where(p => p.PropertyType == typeof(PipelineAction<TContext>));

            var sortedProperties = properties.Select(p => CreateDefinition(p, pipelineSteps)).ToList();
            sortedProperties.Sort(new PipelineStepComparer<TContext>());

            var pipelineScopeOption = sortedProperties.RequiredTransactionScope();

            return new PipelineDefninition<TContext>(sortedProperties, pipelineScopeOption, pipelineSteps.PipelineName);
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