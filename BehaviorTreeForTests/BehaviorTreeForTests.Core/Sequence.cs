using System.Collections.Generic;

namespace Medbullets.Tests.Utils.BehaviorTreeApproach.Core
{
    public class Sequence : IBehaviorTreeElement
    {
        private readonly IBehaviorTreeElement[] sequence;

        public Sequence(params IBehaviorTreeElement[] sequence)
        {
            this.sequence = sequence;
        }

        public IEnumerable<IBehaviorTreeElement> GetAllChild()
        {
            foreach (var behaviorTreeElement in sequence)
            {
                yield return behaviorTreeElement;
            }
        }
    }
}