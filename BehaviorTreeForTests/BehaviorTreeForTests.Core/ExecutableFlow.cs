using System;
using System.Threading.Tasks;

namespace Medbullets.Tests.Utils.BehaviorTreeApproach.Core
{
    public class ExecutableFlow<T> : IExecutableFlow
    {
        private readonly Flow<T> _flow;
        private readonly Func<T> _createContext;

        public ExecutableFlow(Flow<T> flow, Func<T> createContext)
        {
            _flow = flow;
            _createContext = createContext;
        }

        public async Task<FlowExecutionReport> ExecuteAsync(IStepProcessor? processor = null)
        {
            return await _flow.ExecuteAsync(_createContext(), processor ?? new PlainStepProcessor());
        }

        public string Name
        {
            get => $"Flow #{_flow.N}";
        }

        public override string ToString()
        {
            return _flow.ToString();
        }
    }
}