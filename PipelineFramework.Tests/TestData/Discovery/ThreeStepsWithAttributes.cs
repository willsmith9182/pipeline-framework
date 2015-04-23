using PipelinePlusPlus.Core.Attributes;
using PipelinePlusPlus.Core.Steps;

namespace PipelineFramework.Tests.TestData.Discovery
{
    internal class ThreeStepsWithAttributes : PipelineSteps
    {
        public ThreeStepsWithAttributes() : base("ThreeStepsWithAttributes") { }

        [PipelineStep(0)]
        public PipelineStep<DiscoveryTestStepContext> TestStep1 { get; set; }

        [PipelineStep(1)]
        public PipelineStep<DiscoveryTestStepContext> TestStep2 { get; set; }

        [PipelineStep(2)]
        public PipelineStep<DiscoveryTestStepContext> TestStep3 { get; set; }
    }
}