using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Medbullets.CrossCutting.Extensions;

namespace Medbullets.Tests.Utils.BehaviorTreeApproach.Core
{
    public class Flow<T>
    {
        private readonly BehaviorTree.Flow flow;
        private readonly int index;

        public Flow(BehaviorTree.Flow flow, int index)
        {
            this.flow = flow;
            this.index = index;
        }

        public int N => index;

        public List<BehaviorTree.Node> Nodes => flow.Nodes;

        public async Task<FlowExecutionReport> ExecuteAsync(T context, IStepProcessor stepProcessor)
        {
            var report = new FlowExecutionReport()
            {
                FlowDescription = flow.ToString()
            };

            report.Logs.Add($"Start executing flow #{index}.");
            report.Logs.Add(flow.ToString());

            foreach (var node in flow.Nodes)
            {
                report.Logs.Add($"Running {node.Element.GetName()}");

                if (node.Element is IRequirement requirement)
                {
                    report.Logs.Add(requirement.Description);
                }

                try
                {
                    await stepProcessor.BeforeStepProcessed(node.Element);
                    await node.Element.CastTo<IBehavior<T>>().Execute(context);
                    await stepProcessor.AfterStepProcessed(node.Element);
                }
                catch (Exception e)
                {
                    report.Exception = e;

                    report.Suggestions.Add("test to reproduce");
                    report.Suggestions.Add(new BehaviorTree.FailedTestBasedOnScenario<T>(this).GetTest());

                    break;
                }
            }

            report.Logs.Add($"Flow #{index} executed.");

            if (report.Exception == null)
            {
                report.Success = true;
            }

            return report;
        }

        public override string ToString()
        {
            return flow.ToString();
        }
    }
}