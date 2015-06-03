using System;
using System.Diagnostics.Contracts;
using System.Transactions;

namespace PipelinePlusPlus.Core.Attributes
{
    // multiples are not allowed. 
    [AttributeUsage(AttributeTargets.Property)]
    public class PipelineStepAttribute : Attribute
    {
        public PipelineStepAttribute(int order, TransactionScopeOption scope = TransactionScopeOption.Suppress)
        {
            Contract.Requires(order >= 0);
            Order = order;
            TransactionScopeOption = scope;
        }

        public int Order { get; private set; }
        public TransactionScopeOption TransactionScopeOption { get; private set; }
    }
}