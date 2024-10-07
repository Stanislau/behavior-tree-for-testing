using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Medbullets.CrossCutting.Extensions;

namespace Medbullets.Tests.Utils.BehaviorTreeApproach.Core
{
    public static class BehaviorTreeExtensions
    {
        public static IBehaviorTreeElement InThisCase(this IBehaviorTreeElement element, Action action)
        {
            return new StateModification(action, element);
        }

        public static List<Flow<T>> Dump<T>(this List<Flow<T>> flows)
        {
            foreach (var i in flows.Indexed())
            {
                Console.WriteLine($"Flow #{i.current.N}");
                Console.WriteLine(i.current.ToString());
                Console.WriteLine();
            }

            return flows;
        }

        public static async Task ExecuteAll<T>(this List<Flow<T>> flows, Func<T> createContext)
        {
            foreach (var scenario in flows)
            {
                await scenario.ExecuteAsync(createContext(), new PlainStepProcessor());
            }
        }
    }
}