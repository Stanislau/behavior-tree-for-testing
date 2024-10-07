using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Medbullets.CrossCutting.AsyncHelpers;

namespace Medbullets.Tests.Utils.BehaviorTreeApproach.Core
{
    public class BehaviorTree
    {
        private readonly IBehaviorTreeElement element;

        public BehaviorTree(IBehaviorTreeElement element)
        {
            this.element = element;
        }

        public Flows<T> GetAllScenarios<T>()
        {
            var d = DumpV2();

            return new Flows<T>(d.Flows.ToArray(), d.Logs);
        }

        public class Flow
        {
            public List<Node> Nodes { get; private set; } = new();

            public Flow Clone()
            {
                return new Flow()
                {
                    Nodes = Nodes.ToArray().Select(x => x.Clone()).ToList(),
                };
            }

            public bool ExecutedThrough<T>()
            {
                return Nodes.Any(x => typeof(T).IsAssignableFrom(x.Element.GetType()));
            }

            public bool ExecutedThrough(string name)
            {
                return Nodes.Any(x => x.Element.GetName() == name);
            }

            public override string ToString()
            {
                var sb = new StringBuilder();

                foreach (var node in Nodes)
                {
                    sb.AppendLine($"{node.Element.GetName()}");
                }

                return sb.ToString();
            }
        }

        public class FailedTestBasedOnScenario<T>
        {
            private readonly Flow<T> flow;

            public FailedTestBasedOnScenario(Flow<T> flow)
            {
                this.flow = flow;
            }

            public string GetTest()
            {
                var sb = new StringBuilder();

                sb.AppendLine($"[Test, Timeout(1000)]");
                sb.AppendLine($"public async Task FlowN{flow.N}()");
                sb.AppendLine("{");

                sb.AppendLine($"    var c = new {typeof(T).Name}();");

                foreach (var node in flow.Nodes)
                {
                    sb.AppendLine($"    await new {node.Element.GetType().Name}().Execute(c);");
                }

                sb.AppendLine("}");

                return sb.ToString();
            }
        }

        public class Node
        {
            public IBehaviorTreeElement Element { get; set; }
            public bool IsEnd { get; set; }
            public Guid Id { get; set; } = Guid.NewGuid();

            public override bool Equals(object obj)
            {
                return ((Node)obj).Id.Equals(Id);
            }

            public Node Clone()
            {
                return new Node()
                {
                    Element = Element,
                    IsEnd = IsEnd,
                    Id = Id
                };
            }
        }

        public record DumpResult(List<Flow> Flows, List<string> Logs);

        public DumpResult DumpV2()
        {
            var logs = new List<string>();

            var incompleteLists = new List<Flow>()
            {
                new()
                {
                    Nodes =
                    {
                        new Node()
                        {
                            Element = element,
                        }
                    }

                }
            };

            var completeLists = new List<Flow>();

            var total = 100000;
            var iterations = total;

            while (true)
            {
                var flow = incompleteLists.FirstOrDefault();

                if (flow == null)
                {
                    break;
                }

                var processableNode = flow.Nodes.FirstOrDefault(x => x.Element is not IBehavior);

                if (processableNode == null)
                {
                    logs.Add("There are some incompleted flows left.");

                    foreach (var incompleteList in incompleteLists)
                    {
                        logs.Add(incompleteList.ToString());
                    }

                    break;

                    // throw new Exception("Flow is completed, but is not considered as completed.");
                }

                if (processableNode.Element is Sequence sequence)
                {
                    var expandedNodes = new List<Node>();
                    foreach (var child in sequence.GetAllChild())
                    {
                        expandedNodes.Add(new Node()
                        {
                            Element = child
                        });
                    }

                    flow.Nodes.Replace(processableNode, expandedNodes);
                }
                else if (processableNode.Element is Branching branching)
                {
                    incompleteLists.Remove(flow);

                    foreach (var child in branching.GetAllChild())
                    {
                        var candidate = flow.Clone();
                        candidate.Nodes.Replace(processableNode, new List<Node>()
                        {
                            new()
                            {
                                Element = child
                            }
                        });
                        incompleteLists.Add(candidate);
                    }
                }
                else if (processableNode.Element is Merge merging)
                {
                    incompleteLists.Remove(flow);

                    foreach (var child in merging.GetAllChild())
                    {
                        if (child.Satisfies(flow))
                        {
                            var candidate = flow.Clone();
                            candidate.Nodes.Replace(processableNode, new List<Node>()
                            {
                                new()
                                {
                                    Element = child.Element
                                }
                            });
                            incompleteLists.Add(candidate);
                        }
                    }
                }
                else if (processableNode.Element is ConditionalExecution conditionalExecutionV2)
                {
                    if (conditionalExecutionV2.Satisfies(flow))
                    {
                        flow.Nodes.Replace(
                            processableNode,
                            new List<Node>()
                            {
                                new Node()
                                {
                                    Element = conditionalExecutionV2.Element
                                }
                            }
                        );
                    }
                    else
                    {
                        flow.Nodes.Remove(processableNode);
                    }
                }
                else if (processableNode.Element is End end)
                {
                    flow.Nodes.Replace(processableNode, new List<Node>()
                    {
                        new Node()
                        {
                            Element = end.Element,
                            IsEnd = true
                        }
                    });
                }

                foreach (var candidateForCompletion in incompleteLists.ToArray().ToList())
                {
                    if (IsCompleted(candidateForCompletion))
                    {
                        incompleteLists.Remove(candidateForCompletion);

                        completeLists.Add(candidateForCompletion);
                    }
                }

                iterations--;

                if (iterations < 0)
                {
                    System.Diagnostics.Debugger.Break();
                }
            }

            logs.Add($"Behavior tree traversion completed after {total - iterations} iterations.");

            return new DumpResult(completeLists, logs);

            bool IsCompleted(Flow flow)
            {
                if (flow.Nodes.Select(x => x.Element).All(x => x is IBehavior))
                {
                    return true;
                }

                return false;
            }
        }
    }
}