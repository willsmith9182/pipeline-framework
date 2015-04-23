using System.Transactions;
using PipelinePlusPlus.Core;
using PipelinePlusPlus.Core.Attributes;
using PipelinePlusPlus.Core.Steps;

namespace PipelineConsumer.Pipeline
{
    internal class TestPipeline : PipelineSteps
    {
        public TestPipeline()
            : base("TestPipeline")
        {
        }

        [PipelineStep(0, TransactionScopeOption.Required)]
        public PipelineStep<TestStepContext> Step1 { get; set; }

        [PipelineStep(1, TransactionScopeOption.Required)]
        public PipelineStep<TestStepContext> Step2 { get; set; }

        [PipelineStep(2, TransactionScopeOption.Required)]
        public PipelineStep<TestStepContext> Step3 { get; set; }
    }
}