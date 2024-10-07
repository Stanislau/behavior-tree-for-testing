using System.Linq;

namespace Medbullets.Tests.Utils.BehaviorTreeApproach.Core
{
    public class When<T> : ConditionalExecution
    {
        public When(IBehaviorTreeElement element) : base(x => x.ExecutedThrough<T>(), element)
        {
        }
    }

    public class When : ConditionalExecution
    {
        public When(string name, IBehaviorTreeElement element) : base(x => x.ExecutedThrough(name), element)
        {
        }

        public When(INamedBehavior namedBehavior, IBehaviorTreeElement element) : base(x => x.ExecutedThrough(namedBehavior.Name), element)
        {
        }

        public When(INamedBehavior[] namedBehavior, IBehaviorTreeElement element) : base(x => namedBehavior.Any(b => x.ExecutedThrough(b.Name)), element)
        {
        }
    }
}