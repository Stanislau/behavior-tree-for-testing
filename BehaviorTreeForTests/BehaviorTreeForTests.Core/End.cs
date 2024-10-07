namespace Medbullets.Tests.Utils.BehaviorTreeApproach.Core
{
    public class End : IBehaviorTreeElement, IEndOfFlow
    {
        private readonly IBehaviorTreeElement element;

        public End(IBehaviorTreeElement element)
        {
            this.element = element;
        }

        public IBehaviorTreeElement Element => element;
    }
}