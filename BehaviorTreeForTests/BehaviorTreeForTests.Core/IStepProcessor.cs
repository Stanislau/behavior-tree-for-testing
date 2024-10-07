namespace Medbullets.Tests.Utils.BehaviorTreeApproach.Core
{
    public interface IStepProcessor
    {
        Task BeforeStepProcessed(IBehaviorTreeElement step);

        Task AfterStepProcessed(IBehaviorTreeElement step);
    }
}