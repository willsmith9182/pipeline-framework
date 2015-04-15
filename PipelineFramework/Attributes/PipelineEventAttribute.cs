using System;
using Pipeline.Support_Code;

namespace Pipeline.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PipelineEventAttribute : Attribute
    {
        public PipelineEventAttribute()
        {
            Order = 0;
            TransactionScopeOption = TransactionScopeOption.Suppress;
        }

        public int Order { get; private set; }
        public TransactionScopeOption TransactionScopeOption { get; private set; }
    }
}