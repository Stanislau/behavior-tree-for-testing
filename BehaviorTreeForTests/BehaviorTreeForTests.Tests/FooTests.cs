using BehaviorTreeForTests.Core;
using NUnit.Framework;

namespace BehaviorTreeForTests.Tests
{
    /*
     * @todo #9:30m/DevOps Setup sonar cube for coverage.
     */

    [TestFixture]
    public class FooTests
    {
        [Test]
        public void Smoke()
        {
            Assert.That(new Foo().ToString(), Is.EqualTo("bar"));
        }
    }
}
