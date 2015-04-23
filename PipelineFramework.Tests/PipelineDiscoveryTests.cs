using System.Linq;
using System.Transactions;
using NUnit.Framework;
using PipelineFramework.Tests.TestData.Discovery;
using PipelinePlusPlus.Core;
using PipelinePlusPlus.Core.Discovery;
using PipelinePlusPlus.Core.Exceptions;
using PipelinePlusPlus.Core.Steps;

namespace PipelineFramework.Tests
{
    [TestFixture]
    public class PipelineDiscoveryTests
    {
        [Test]
        public void WhenCallingDiscoverWithValidSteps_ShouldProduceCorrectDefinition()
        {
            // arrange
            var steps = new ThreeStepsWithAttributes();

            var sut = new PipelineDiscovery();

            // act
            PipelineDefinition<DiscoveryTestStepContext> result = null;

            Assert.DoesNotThrow(() => { result = sut.Discover<DiscoveryTestStepContext>(steps); });

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(steps.TestStep1, Is.Not.Null);
            Assert.That(steps.TestStep2, Is.Not.Null);
            Assert.That(steps.TestStep3, Is.Not.Null);
            Assert.That(result.PipelineName, Is.EqualTo(steps.PipelineName));
            Assert.That(result.PipelineScopeOption, Is.EqualTo(TransactionScopeOption.Suppress));
            Assert.That(result.Actions.Count, Is.EqualTo(3));

            var castActions = result.Actions.Cast<PipelineStepDefinintion<DiscoveryTestStepContext>>().ToArray();

            Assert.That(castActions, Is.Not.Empty);
            Assert.That(castActions.Count(), Is.EqualTo(result.Actions.Count));

            // happy path, if the first item is mapped correctly i'm gonna assume the rest are. 
            // other tests will cover variances in the attribute usage on steps
            var step1 = castActions[0];

            Assert.That(step1.Attr, Is.Not.Null);
            Assert.That(step1.Attr.Order, Is.EqualTo(0));
            Assert.That(step1.Attr.TransactionScopeOption, Is.EqualTo(TransactionScopeOption.Suppress));
            // is the instance i hold in my def the same ref as the instance on the steps.  :)
            Assert.That(step1.Step, Is.SameAs(steps.TestStep1));
            Assert.That(step1.StepName, Is.EqualTo("TestStep1"));

        }

        [Test]
        public void WhenCallingDiscoverWithValidStepsWhereOneIsAlreadyInstantiated_ShouldInstantiateOtherSteps()
        {
            // arrange
            var steps = new ThreeStepsWithAttributes
            {
                TestStep3 = new PipelineStep<DiscoveryTestStepContext>()
            };

            var step3 = steps.TestStep3;

            var sut = new PipelineDiscovery();

            // act
            PipelineDefinition<DiscoveryTestStepContext> result = null;

            Assert.DoesNotThrow(() => { result = sut.Discover<DiscoveryTestStepContext>(steps); });

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(steps.TestStep1, Is.Not.Null);
            Assert.That(steps.TestStep2, Is.Not.Null);

            // are all the references all the same!
            Assert.That(step3, Is.SameAs(steps.TestStep3));
            Assert.That(step3, Is.SameAs(((PipelineStepDefinintion<DiscoveryTestStepContext>)result.Actions.Last()).Step));
        }

        [Test]
        public void WhenCallingDiscoverWithAStepWithAttributeScopeOptionRequired_ShouldDefineCorrectTranScope()
        {
            // arrange
            var steps = new ThreeStepsWithRequiredTranScope();
            
            var sut = new PipelineDiscovery();

            // act
            PipelineDefinition<DiscoveryTestStepContext> result = null;

            Assert.DoesNotThrow(() => { result = sut.Discover<DiscoveryTestStepContext>(steps); });

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.PipelineScopeOption, Is.EqualTo(TransactionScopeOption.Required));
        }

        [Test]
        public void WhenCallingDiscoverWithAStepWithAttributeScopeOptionRequireNew_ShouldDefineCorrectTranScope()
        {
            // arrange
            var steps = new ThreeStepsWithRequiresNewTranScope();

            var sut = new PipelineDiscovery();

            // act
            PipelineDefinition<DiscoveryTestStepContext> result = null;

            Assert.DoesNotThrow(() => { result = sut.Discover<DiscoveryTestStepContext>(steps); });

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.PipelineScopeOption, Is.EqualTo(TransactionScopeOption.RequiresNew));
        }

        [Test]
        public void WhenCallingDiscoverWithStepsForWrongContext_ShouldThrowException()
        {
            // arrange
            var steps = new ThreeStepsForWrongContext();

            var sut = new PipelineDiscovery();

            // act
            var ex = Assert.Throws<PipelineDicoveryException>(() => { var result = sut.Discover<DiscoveryTestStepContext>(steps); });

            // assert
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Is.EqualTo(string.Format("No properties found on the Pipeline Definition '{0}'. Discovery Aborted", steps.PipelineName)));

        }

        [Test]
        public void WhenCallingDiscoverWithStepsThatHaveInvalidSequenceOrder_ShouldThrowException()
        {
            // arrange
            var steps = new ThreeStepsWithInvalidOrder();

            var sut = new PipelineDiscovery();

            // act
            var ex = Assert.Throws<PipelineDicoveryException>(() => { var result = sut.Discover<DiscoveryTestStepContext>(steps); });

            // assert
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Is.EqualTo("The order value in the PipelineStepAttribute has duplicates. Please review your steps and ensure that the SequenceOrder value is set correctly and not duplicated per task."));
        }

        [Test]
        public void WhenCallingDiscoverWithStepsMissingAnAttribute_ShouldThrowException()
        {
            // arrange
            var steps = new ThreeStepsWithMissingAttribute();

            var sut = new PipelineDiscovery();

            // act
            var ex = Assert.Throws<PipelineDicoveryException>(() => { var result = sut.Discover<DiscoveryTestStepContext>(steps); });

            // assert
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Is.EqualTo(string.Format("Step '{0}' has no PipelineStepAttribute defined. Please review the pipeline definition '{1}'", "TestStep3", steps.PipelineName)));
        }
    }
}
