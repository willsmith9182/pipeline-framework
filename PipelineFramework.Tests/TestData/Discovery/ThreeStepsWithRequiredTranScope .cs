using System.Transactions;
using PipelinePlusPlus.Core;
using PipelinePlusPlus.Core.Attributes;
using PipelinePlusPlus.Core.Steps;

namespace PipelineFramework.Tests.TestData.Discovery
{
    internal class ThreeStepsWithRequiredTranScope : PipelineSteps
    {


        public ThreeStepsWithRequiredTranScope()
            : base("ThreeStepsWithRequiredTranScope")
        {
        }

        [PipelineStep(0, TransactionScopeOption.Required)]
        public PipelineStep<DiscoveryTestStepContext> TestStep1 { get; set; }
        [PipelineStep(1)]
        public PipelineStep<DiscoveryTestStepContext> TestStep2 { get; set; }
        [PipelineStep(2)]
        public PipelineStep<DiscoveryTestStepContext> TestStep3 { get; set; }
    }
}