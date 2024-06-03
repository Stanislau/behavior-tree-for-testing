using BehaviorTreeForTests.Core;
using NUnit.Framework;

namespace BehaviorTreeForTests.Tests
{
    /*
     * @todo #4:30m/Arch Setup automation pipeline to check pull request health.
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
