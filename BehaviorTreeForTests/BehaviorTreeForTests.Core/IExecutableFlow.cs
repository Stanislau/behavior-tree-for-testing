using System.Threading.Tasks;

namespace Medbullets.Tests.Utils.BehaviorTreeApproach.Core
{
    public interface IExecutableFlow
    {
        Task<FlowExecutionReport> ExecuteAsync(IStepProcessor? processor = null);

        string Name { get; }
    }
}