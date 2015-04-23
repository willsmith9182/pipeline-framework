using PipelinePlusPlus.Core.Attributes;
using PipelinePlusPlus.Core.Context;
using PipelinePlusPlus.Core.Steps;

namespace PipelineFramework.Tests.TestData.Discovery
{
    internal class ThreeStepsForWrongContext : PipelineSteps
    {
        public ThreeStepsForWrongContext() : base("ThreeStepsForWrongContext") { }

        [PipelineStep(0)]
        public PipelineStep<TheWrongStepContext> TestStep1 { get; set; }

        [PipelineStep(1)]
        public PipelineStep<TheWrongStepContext> TestStep2 { get; set; }

        [PipelineStep(2)]
        public PipelineStep<TheWrongStepContext> TestStep3 { get; set; }

        internal class TheWrongStepContext : PipelineStepContext {}
    }
}