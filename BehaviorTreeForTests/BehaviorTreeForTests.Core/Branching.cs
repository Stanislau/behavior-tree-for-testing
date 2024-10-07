using System.Collections.Generic;

namespace Medbullets.Tests.Utils.BehaviorTreeApproach.Core
{
    public class Branching : IBehaviorTreeElement
    {
        private readonly IBehaviorTreeElement[] branching;

        public Branching(params IBehaviorTreeElement[] branching)
        {
            this.branching = branching;
        }

        public IEnumerable<IBehaviorTreeElement> GetAllChild()
        {
            foreach (var behaviorTreeElement in branching)
            {
                yield return behaviorTreeElement;
            }
        }
    }
}