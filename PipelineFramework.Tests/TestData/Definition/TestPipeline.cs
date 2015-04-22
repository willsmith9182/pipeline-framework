using PipelinePlusPlus.Core;

namespace PipelineFramework.Tests.TestData.Definition
{
    public class TestPipeline : PipelineSteps
    {
        public TestPipeline()
            : base(TestUtils.PipelineNameForTest)
        {
        }
    }
}
