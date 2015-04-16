using System;
using System.Transactions;

namespace PipelinePlusPlus.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PipelineStepAttribute : Attribute
    {
        public PipelineStepAttribute(int order, TransactionScopeOption scope = TransactionScopeOption.Suppress)
        {
            Order = order;
            TransactionScopeOption = scope;
        }

        public int Order { get; private set; }
        public TransactionScopeOption TransactionScopeOption { get; private set; }
    }
}