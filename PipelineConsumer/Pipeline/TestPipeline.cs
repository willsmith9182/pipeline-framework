using System.Transactions;
using PipelinePlusPlus.Attributes;
using PipelinePlusPlus.Core;

namespace PipelineConsumer.Pipeline
{
    internal class TestPipeline : PipelineSteps
    {
        public TestPipeline()
            : base("TestPipeline")
        {
        }

        [PipelineStep(0, TransactionScopeOption.Required)]
        public PipelineStep<TestContext> Step1 { get; set; }

        [PipelineStep(1, TransactionScopeOption.Required)]
        public PipelineStep<TestContext> Step2 { get; set; }

        [PipelineStep(2, TransactionScopeOption.Required)]
        public PipelineStep<TestContext> Step3 { get; set; }
    }
}