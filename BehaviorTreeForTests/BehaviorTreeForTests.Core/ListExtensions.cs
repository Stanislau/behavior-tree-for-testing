using System.Collections.Generic;
using Medbullets.CrossCutting.Extensions;

namespace Medbullets.Tests.Utils.BehaviorTreeApproach.Core
{
    public static class ListExtensions
    {
        public static List<T> Replace<T>(this List<T> items, T from, IEnumerable<T> to)
        {
            var i = items.IndexOfOrDefault(x => ReferenceEquals(x, from) || x.Equals(from));

            if (i.HasValue)
            {
                items.RemoveAt(i.Value);

                items.InsertRange(i.Value, to);
            }

            return items;
        }
    }
}