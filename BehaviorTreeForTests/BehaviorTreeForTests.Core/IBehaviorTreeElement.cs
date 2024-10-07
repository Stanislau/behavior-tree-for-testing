namespace Medbullets.Tests.Utils.BehaviorTreeApproach.Core
{
    public interface IBehaviorTreeElement
    {
        public string GetName()
        {
            if (this is INamedBehavior namedBehavior)
            {
                return namedBehavior.Name;
            }

            return this.GetType().Name;
        }
    }
}