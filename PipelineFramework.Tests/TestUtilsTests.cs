using System.Collections.Specialized;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace PipelineFramework.Tests
{
    [TestFixture]
    public class TestUtilsTests
    {
        [Test]
        public void WhenUsingNameValueCollectionConstraintWithCorrectData_ShouldPass()
        {
            var expected = new NameValueCollection
            {
                {
                    "Key1", "Data1"
                },
                {
                    "Key2", "Data2"
                },
                {
                    "Key3", "Data3"
                }
            };
            var actual = new NameValueCollection
            {
                {
                    "Key1", "Data1"
                },
                {
                    "Key2", "Data2"
                },
                {
                    "Key3", "Data3"
                }
            };


            var sut = new TestUtils.NameValueCollectionConstraint(expected);

            var result = sut.Matches(actual);

            Assert.That(result, Is.True);
        }

        [Test]
        public void WhenUsingNameValueCollectionConstraintWithMissingKey_ShouldFail()
        {
            var expected = new NameValueCollection
            {
                {
                    "Key1", "Data1"
                },
                {
                    "Key2", "Data2"
                },
                {
                    "Key3", "Data3"
                }
            };
            var actual = new NameValueCollection
            {
                {
                    "Key1", "Data1"
                },
                {
                    "Key3", "Data3"
                }
            };


            var sut = new TestUtils.NameValueCollectionConstraint(expected);

            var result = sut.Matches(actual);

            Assert.That(result, Is.False);
        }

        [Test]
        public void WhenUsingNameValueCollectionConstraintWithMismatchingData_ShouldFail()
        {
            var expected = new NameValueCollection
            {
                {
                    "Key1", "Data1"
                },
                {
                    "Key2", "Data2"
                },
                {
                    "Key3", "Data3"
                }
            };
            var actual = new NameValueCollection
            {
                {
                    "Key1", "Data1"
                },
                {
                    "Key2", "Data2"
                },
                {
                    "Key3", "Data4"
                }
            };


            var sut = new TestUtils.NameValueCollectionConstraint(expected);

            var result = sut.Matches(actual);

            Assert.That(result, Is.False);
        }

        [Test]
        public void WhenUsingNameValueCollectionConstraint_ShouldCallWriterCorrectly()
        {
            var msgWriter = new Mock<MessageWriter>();

            msgWriter.Setup(w => w.WriteExpectedValue(It.IsAny<object>()));

            var expected = new NameValueCollection
            {
                {
                    "Key1", "Data1"
                },
                {
                    "Key2", "Data2"
                },
                {
                    "Key3", "Data3"
                }
            };
            var actual = new NameValueCollection
            {
                {
                    "Key1", "Data1"
                },
                {
                    "Key2", "Data2"
                },
                {
                    "Key3", "Data3"
                }
            };

            var sut = new TestUtils.NameValueCollectionConstraint(expected);

            sut.WriteDescriptionTo(msgWriter.Object);

            // verify call and value passed to writer. 
            msgWriter.Verify(w => w.WriteExpectedValue(It.Is<object>(o => o is NameValueCollection && ((NameValueCollection) o).Equals(expected))), Times.Exactly(1));
        }
    }
}