using System;
using System.Transactions;

namespace PipelinePlusPlus.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PipelineStepAttribute : Attribute
    {
        // why am i defined where i am?
        //http://stackoverflow.com/a/1190760 - to restrict my usage to only people who inherit pipeline steps! 
        public PipelineStepAttribute(int order, TransactionScopeOption scope = TransactionScopeOption.Suppress)
        {
            Order = order;
            TransactionScopeOption = scope;
        }
        
        public int Order { get; private set; }
        public TransactionScopeOption TransactionScopeOption { get; private set; }
    }
}