using BehaviorTreeForTests.Core;
using NUnit.Framework;

namespace BehaviorTreeForTests.Tests
{
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
