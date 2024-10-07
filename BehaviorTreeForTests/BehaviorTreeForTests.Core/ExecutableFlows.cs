using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medbullets.Tests.Utils.BehaviorTreeApproach.Core
{
    public class ExecutableFlows<T> : IExecutableFlows
    {
        private readonly Flows<T> _flows;
        private readonly ExecutableFlow<T>[] _executableFlows;

        public ExecutableFlows(Flows<T> flows, Func<T> createContext)
        {
            _flows = flows;
            _executableFlows = flows.ToArray().Select(x => new ExecutableFlow<T>(x, createContext)).ToArray();
        }

        public IEnumerable<string> Logs => _flows.Logs;

        public async Task<FlowExecutionReport[]> ExecuteAll()
        {
            var reports = new List<FlowExecutionReport>();

            foreach (var flow in _executableFlows)
            {
                reports.Add(await flow.ExecuteAsync());
            }

            return reports.ToArray();
        }

        public IEnumerator<IExecutableFlow> GetEnumerator()
        {
            return _executableFlows.ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}