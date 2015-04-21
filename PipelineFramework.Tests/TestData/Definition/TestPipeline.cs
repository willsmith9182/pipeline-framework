using PipelinePlusPlus.Core;

namespace PipelineFramework.Tests.TestData.Definition
{
    public class TestPipeline : PipelineSteps<TestPIpelineContext>
    {
        public TestPipeline()
            : base(TestUtils.PipelineNameForTest)
        {
        }
    }
}
