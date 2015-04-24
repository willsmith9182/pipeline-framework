using PipelinePlusPlus.Core.Steps;

namespace PipelineFramework.Tests.TestData.Builder
{
    public class TestPipeline : PipelineSteps
    {
        public TestPipeline()
            : base(TestUtils.PipelineNameForTest)
        {
        }
    }
}
