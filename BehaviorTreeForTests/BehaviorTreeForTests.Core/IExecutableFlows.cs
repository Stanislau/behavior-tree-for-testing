using System.Collections.Generic;
using System.Threading.Tasks;

namespace Medbullets.Tests.Utils.BehaviorTreeApproach.Core
{
    public interface IExecutableFlows : IEnumerable<IExecutableFlow>
    {
        IEnumerable<string> Logs { get; }

        Task<FlowExecutionReport[]> ExecuteAll();
    }
}