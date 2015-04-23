using PipelinePlusPlus.Core.Attributes;
using PipelinePlusPlus.Core.Steps;

namespace PipelineFramework.Tests.TestData.Discovery
{
    internal class ThreeStepsWithMissingAttribute : PipelineSteps
    {
        public ThreeStepsWithMissingAttribute() : base("ThreeStepsWithMissingAttribute") { }

        [PipelineStep(0)]
        public PipelineStep<DiscoveryTestStepContext> TestStep1 { get; set; }

        [PipelineStep(1)]
        public PipelineStep<DiscoveryTestStepContext> TestStep2 { get; set; }

        public PipelineStep<DiscoveryTestStepContext> TestStep3 { get; set; }
    }
}