using System.Threading.Tasks;

namespace Medbullets.Tests.Utils.BehaviorTreeApproach.Core
{
    public interface IBehavior : IBehaviorTreeElement
    {

    }

    public interface IBehavior<in T> : IBehavior
    {
        Task Execute(T context);
    }
}