using System;

namespace Medbullets.Tests.Utils.BehaviorTreeApproach.Core
{
    public class ConditionalExecution : IBehaviorTreeElement, IStateControlNode
    {
        public IBehaviorTreeElement Element { get; }
        private readonly Func<BehaviorTree.Flow, bool> condition;

        public ConditionalExecution(Func<BehaviorTree.Flow, bool> condition, IBehaviorTreeElement element)
        {
            Element = element;
            this.condition = condition;
        }

        public bool Satisfies(BehaviorTree.Flow flow)
        {
            return condition(flow);
        }
    }
}