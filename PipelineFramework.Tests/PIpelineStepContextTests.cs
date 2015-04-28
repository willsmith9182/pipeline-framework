using NUnit.Framework;
using PipelineFramework.Tests.TestData.Builder;

namespace PipelineFramework.Tests
{
    [TestFixture]
    public class PipelineStepContextTests
    {

        [Test]
        public void WhenUsingPipelineStepContext_ShouldInitialiseToValidState()
        {
            // Arrange 

            var sut = new TestPipelineStepContext();

            // Act
            // no act. 

            // Assert
            Assert.That(sut.CancelExecution, Is.False);
            Assert.That(sut.CancelReason, Is.Null);
        }

        [Test]
        public void WhenUsingPipelineStepContextCallingCancel_ShouldSetFlagCorrectlyAndCreateException()
        {
            // Arrange 
            const string testCancellationReason = "Cancelling during testing";
            const string testModuleName = "TestModule";

            var sut = new TestPipelineStepContext();

            // Act
            sut.Cancel(testCancellationReason, testModuleName);

            // Assert
            Assert.That(sut.CancelExecution, Is.True);
            Assert.That(sut.CancelReason, Is.Not.Null);
            Assert.That(sut.CancelReason.Message, Is.StringContaining(testCancellationReason));
            Assert.That(sut.CancelReason.Message, Is.StringContaining(testModuleName));

        }
    }
}
