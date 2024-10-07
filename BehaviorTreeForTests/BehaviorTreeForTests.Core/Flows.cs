using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medbullets.Tests.Utils.BehaviorTreeApproach.Core
{
    public class Flows<T> : IEnumerable<Flow<T>>
    {
        private readonly Flow<T>[] _flows;
        private readonly IEnumerable<string> _logs;

        public Flows(BehaviorTree.Flow[] flows, IEnumerable<string> logs)
        {
            _flows = flows.Where(x => x.Nodes.All(y => y.Element is IBehavior<T>)).Select((x, index) => new Flow<T>(x, index + 1)).ToArray();
            _logs = logs;
        }

        public IEnumerable<string> Logs => _logs;

        public IEnumerator<Flow<T>> GetEnumerator()
        {
            return _flows.ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IExecutableFlows ToExecutable(Func<T> createContext)
        {
            return new ExecutableFlows<T>(this, createContext);
        }

        public async Task<FlowExecutionReport[]> ExecuteAll(Func<T> createContext, IStepProcessor? processor = null)
        {
            var reports = new List<FlowExecutionReport>();

            foreach (var scenario in _flows)
            {
                reports.Add(await scenario.ExecuteAsync(createContext(), processor ?? new PlainStepProcessor()));
            }

            return reports.ToArray();
        }
    }
}