using System;

namespace Medbullets.Tests.Utils.BehaviorTreeApproach.Core
{
    public class StateModification : IBehaviorTreeElement, IStateControlNode
    {
        public IBehaviorTreeElement Element { get; }
        private readonly Action modification;

        public StateModification(Action modification, IBehaviorTreeElement element)
        {
            Element = element;
            this.modification = modification;
        }

        public void ModifyState()
        {
            modification();
        }
    }
}