using System.Transactions;
using PipelinePlusPlus.Core;
using PipelinePlusPlus.Core.Attributes;
using PipelinePlusPlus.Core.Steps;

namespace PipelineFramework.Tests.TestData.Discovery
{
    internal class ThreeStepsWithRequiresNewTranScope : PipelineSteps
    {


        public ThreeStepsWithRequiresNewTranScope()
            : base("ThreeStepsWithRequiresNewTranScope")
        {
        }

        [PipelineStep(0, TransactionScopeOption.RequiresNew)]
        public PipelineStep<DiscoveryTestStepContext> TestStep1 { get; set; }
        [PipelineStep(1)]
        public PipelineStep<DiscoveryTestStepContext> TestStep2 { get; set; }
        [PipelineStep(2)]
        public PipelineStep<DiscoveryTestStepContext> TestStep3 { get; set; }
    }
}