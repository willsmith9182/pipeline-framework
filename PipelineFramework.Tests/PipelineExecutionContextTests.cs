using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Transactions;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using PipelineFramework.Tests.TestData.Builder;
using PipelinePlusPlus.Core.Context;
using PipelinePlusPlus.Core.Discovery;
using PipelinePlusPlus.Core.Exceptions;
using PipelinePlusPlus.Core.Steps;

namespace PipelineFramework.Tests
{
    [TestFixture]
    public class PipelineExecutionContextTests
    {
        private Mock<IPipelineDefinition<TestPipelineStepContext>> _mockDefinition;
        private IReadOnlyCollection<IPipelineStepDefinintion<TestPipelineStepContext>> _readonlyActions;
        private Func<PipelineException, bool> _onErrorHandler = e => true;

        private void Setup(IEnumerable<IPipelineStepDefinintion<TestPipelineStepContext>> actions, TransactionScopeOption pipelineScopeOption, string pipelineName)
        {
            _mockDefinition = new Mock<IPipelineDefinition<TestPipelineStepContext>>(MockBehavior.Strict);

            _readonlyActions = new ReadOnlyCollection<IPipelineStepDefinintion<TestPipelineStepContext>>(actions.ToList());

            _mockDefinition.Setup(m => m.Actions).Returns(_readonlyActions);
            _mockDefinition.Setup(m => m.PipelineName).Returns(pipelineName);
            _mockDefinition.Setup(m => m.PipelineScopeOption).Returns(pipelineScopeOption);

        }

        private PipelineExecutionContext<TestPipelineStepContext> CreateSut(string pipelineName, Action postSetup = null)
        {
            return CreateSut(pipelineName, TransactionScopeOption.Suppress, postSetup);
        }

        private PipelineExecutionContext<TestPipelineStepContext> CreateSut(string pipelineName, TransactionScopeOption pipelineScopeOption, Action postSetup = null)
        {
            return CreateSut(pipelineName, pipelineScopeOption, new IPipelineStepDefinintion<TestPipelineStepContext>[] { }, postSetup);
        }

        private PipelineExecutionContext<TestPipelineStepContext> CreateSut(string pipelineName, TransactionScopeOption pipelineScopeOption, IEnumerable<IPipelineStepDefinintion<TestPipelineStepContext>> actions, Action postSetup = null)
        {
            Setup(actions, pipelineScopeOption, pipelineName);
            if (postSetup != null)
                postSetup();

            return new PipelineExecutionContext<TestPipelineStepContext>(_mockDefinition.Object, _onErrorHandler);
        }

        [Test]
        public void WhenUsingExecutionContext_ShouldInitialiseToValidState()
        {
            // Arrange
            var expectedSteps = new List<IPipelineStepDefinintion<TestPipelineStepContext>>
            {
                It.IsAny<IPipelineStepDefinintion<TestPipelineStepContext>>(),
                It.IsAny<IPipelineStepDefinintion<TestPipelineStepContext>>()
            };
            const TransactionScopeOption expectedScope = TransactionScopeOption.RequiresNew;

            var sut = CreateSut(TestUtils.PipelineNameForTest, expectedScope, expectedSteps);

            // Act
            // nope

            // Assert
            Assert.That(sut.PipelineName, Is.EqualTo(TestUtils.PipelineNameForTest));
            Assert.That(sut.Steps.Count, Is.EqualTo(expectedSteps.Count));
            Assert.That(sut.PipelineScope, Is.EqualTo(expectedScope));
            Assert.That(sut.PipelineStageExecuted, Is.Null);
            Assert.That(sut.PipelineStageExecuting, Is.Null);
            Assert.That(sut.CancelExecution, Is.False);
            Assert.That(sut.StepContext, Is.Null);
            Assert.That(sut.Exceptions, Is.Empty);
        }

        [Test]
        public void WhenUsingExecutionContext_ShouldCallOnErrorHandlerWhenExceptionIsRegistered()
        {
            // Arrange
            var errorHandlerCalled = false;

            var sut = CreateSut(TestUtils.PipelineNameForTest, () =>
            {
                _onErrorHandler = e =>
                {
                    errorHandlerCalled = true;
                    return true;
                };
            });

            var ex = new InvalidOperationException("Test Exception");

            // Act
            sut.RegisterPipelineError(ex, "TestModule");

            // Assert

            Assert.That(errorHandlerCalled, Is.True);
        }

        [Test]
        public void WhenUsingExecutionContext_ShouldNotCancelExecutionOnErrorIfHandlerReturnsFalseAndExecutionNotAlreadyCancelled()
        {
            // Arrange
            var errorHandlerCalled = false;

            var sut = CreateSut(TestUtils.PipelineNameForTest, () =>
            {
                _onErrorHandler = e =>
                {
                    errorHandlerCalled = true;
                    return false;
                };
            });

            var ex = new InvalidOperationException("Test Exception");
            Assert.That(sut.CancelExecution, Is.False);
            // Act
            sut.RegisterPipelineError(ex, "TestModule");

            // Assert
            Assert.That(sut.CancelExecution, Is.False);
            Assert.That(errorHandlerCalled, Is.True);
        }

        [Test]
        public void WhenUsingExecutionContext_ShouldLogExceptionRegardlessOfErrorHandlerReturn()
        {
            // Arrange
            var sut = CreateSut(TestUtils.PipelineNameForTest, () =>
            {
                _onErrorHandler = e =>
                {
                    if (e.InnerException is InvalidOperationException)
                        return false;
                    return true;
                };
            });

            var ex1 = new InvalidOperationException("Test Exception");
            var ex2 = new ArgumentException("Test Exception");

            // Act
            sut.RegisterPipelineError(ex1, "TestModule");
            sut.RegisterPipelineError(ex2, "TestModule");

            // Assert
            Assert.That(sut.Exceptions, Is.Not.Empty);
            Assert.That(sut.Exceptions.Count, Is.EqualTo(2));

        }

        [Test]
        public void WhenUsingExecutionContext_ShouldNotResetCancelExecutionFlagIfPreviousExceptionHaltedExecution()
        {
            // Arrange
            var sut = CreateSut(TestUtils.PipelineNameForTest, () =>
            {
                // this handler will halt on first error type, 
                // will not halt on 2nd
                _onErrorHandler = e =>
                {
                    if (e.InnerException is InvalidOperationException)
                        return true;
                    return false;
                };
            });

            var ex1 = new InvalidOperationException("Test Exception");
            var ex2 = new ArgumentException("Test Exception");

            // Act
            sut.RegisterPipelineError(ex1, "TestModule");
            // check that execution flag set
            Assert.That(sut.CancelExecution, Is.True);

            // register another error that doens't cause pipeline to cancel
            sut.RegisterPipelineError(ex2, "TestModule");

            // Assert that the pipeline is still cancelled. 
            Assert.That(sut.CancelExecution, Is.True);

        }
        // reverse order of test above. 
        // ensures that it isn't jsut defaulting to cencel execution.
        [Test]
        public void WhenUsingExecutionContext_ShouldSetCancelExecutionFlagCorrectly()
        {
            // Arrange

            var sut = CreateSut(TestUtils.PipelineNameForTest, () =>
            {
                // this handler will halt on first error type, 
                // will not halt on 2nd
                _onErrorHandler = e =>
                {
                    if (e.InnerException is ArgumentException)
                        return false;
                    return true;
                };
            });

            var ex1 = new InvalidOperationException("Test Exception");
            var ex2 = new ArgumentException("Test Exception");

            // Act
            sut.RegisterPipelineError(ex2, "TestModule");
            // check that execution flag set
            Assert.That(sut.CancelExecution, Is.False);

            // register another error that doens't cause pipeline to cancel
            sut.RegisterPipelineError(ex1, "TestModule");

            // Assert that the pipeline is still cancelled. 
            Assert.That(sut.CancelExecution, Is.True);

        }

        [Test]
        public void WhenUsingExecutionContext_ShouldResetContextWhenNewStepContextIsSet()
        {
            // Arrange
            var sut = CreateSut(TestUtils.PipelineNameForTest, () =>
            {
                _onErrorHandler = e => true;
            });

            var ex1 = new InvalidOperationException("Test Exception");
            var ex2 = new ArgumentException("Test Exception");
            var exisitngCxt = new TestPipelineStepContext();

            // Act
            // register initial context
            sut.StepContext = exisitngCxt;

            // assert correct state
            Assert.That(sut.CancelExecution, Is.False);
            Assert.That(sut.Exceptions, Is.Empty);
            Assert.That(sut.StepContext, Is.SameAs(exisitngCxt));

            // cause some errors/change state of context
            sut.RegisterPipelineError(ex1, "TestModule");
            sut.RegisterPipelineError(ex2, "TestModule");

            // assert state changed
            Assert.That(sut.CancelExecution, Is.True);
            Assert.That(sut.Exceptions, Is.Not.Empty);

            // create another new context
            var newCxt = new TestPipelineStepContext();
            sut.StepContext = newCxt;

            // Assert - that changed state was reset as expected
            Assert.That(sut.CancelExecution, Is.False);
            Assert.That(sut.Exceptions, Is.Empty);
            Assert.That(sut.StepContext, Is.SameAs(newCxt));
        }

        [Test]
        public void WhenUsingExecutionContext_ShouldCancelExecutionWhenCancelCurrentExecutionIsCalled()
        {
            // Arrange
            var sut = CreateSut(TestUtils.PipelineNameForTest);

            // Act
            Assert.That(sut.CancelExecution, Is.False);
            sut.CancelCurrentExecution();

            // Assert 
            Assert.That(sut.CancelExecution, Is.True);
            Assert.That(sut.Exceptions, Is.Empty);
        }

        [Test]
        public void WhenUsingExecutionContext_ShouldCancelExecutionAndRegisterExceptionWhenCancelCurrentExecutionIsCalledWithException()
        {
            // Arrange
            var sut = CreateSut(TestUtils.PipelineNameForTest);
            
            // Act
            Assert.That(sut.CancelExecution, Is.False);
            sut.CancelCurrentExecution(new PipelineException("A Pipeline Exception", "Test Module"));

            // Assert 
            Assert.That(sut.CancelExecution, Is.True);
            Assert.That(sut.Exceptions, Is.Not.Empty);
            Assert.That(sut.Exceptions.Count, Is.EqualTo(1));
        }
    }
}
