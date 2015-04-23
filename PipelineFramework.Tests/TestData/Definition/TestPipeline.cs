using PipelinePlusPlus.Core;
using PipelinePlusPlus.Core.Steps;

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
