using NUnit.Framework;
using PipelinePlusPlus.Attributes;
using PipelinePlusPlus.Core;

namespace PipelineFramework.Tests
{
    [TestFixture]
    public class PipelineDiscoveryTests
    {
        internal class DiscoveryTestContext
        : PipelineContext
        {

        }
        internal class DiscoveryTestPipeline : PipelineSteps
        {
            public DiscoveryTestPipeline()
                : base("DiscoveryTests")
            {
            }

            [PipelineStep(0)]
            public PipelineStep<DiscoveryTestContext> TestStep1 { get; set; }
            [PipelineStep(1)]
            public PipelineStep<DiscoveryTestContext> TestStep2 { get; set; }
            [PipelineStep(2)]
            public PipelineStep<DiscoveryTestContext> TestStep3 { get; set; }
        }

    }
}
