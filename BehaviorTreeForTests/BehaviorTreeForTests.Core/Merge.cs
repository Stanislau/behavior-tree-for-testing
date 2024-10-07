using System.Collections.Generic;

namespace Medbullets.Tests.Utils.BehaviorTreeApproach.Core
{
    public class Merge : IBehaviorTreeElement
    {
        private readonly ConditionalExecution[] branching;

        public Merge(params ConditionalExecution[] branching)
        {
            this.branching = branching;
        }

        public IEnumerable<ConditionalExecution> GetAllChild()
        {
            foreach (var behaviorTreeElement in branching)
            {
                yield return behaviorTreeElement;
            }
        }
    }
}