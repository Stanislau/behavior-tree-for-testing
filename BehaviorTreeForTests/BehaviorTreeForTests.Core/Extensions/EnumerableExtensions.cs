using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Medbullets.CrossCutting.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool EqualsConsideringOrder<T>(this IEnumerable<T> self, IEnumerable<T> another)
        {
            if (self.Count() != another.Count())
            {
                return false;
            }

            for (int i = 0; i < self.Count(); i++)
            {
                if (self.ElementAt(i).Equals(another.ElementAt(i)) == false)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsOutOfRange<T>(this int index, IEnumerable<T> enumerable)
        {
            return index < 0 || index >= enumerable.Count();
        }

        public static IEnumerable<T> SelfOrEmpty<T>(IEnumerable<T> enumerable)
        {
            return enumerable ?? Enumerable.Empty<T>();
        }

        public static IEnumerable<T> RemoveAllAt<T>(this IEnumerable<T> self, IEnumerable<int> indexes)
        {
            foreach (var item in self.Indexed())
            {
                if (indexes.Contains(item.index) == false)
                {
                    yield return item.current;
                }
            }
        }

        public static bool ContainsAnyOfSubstrings(this string self, IEnumerable<string> target)
        {
            var s = self.ToLowerInvariant();

            return target.Any(x => s.Contains(x.ToLowerInvariant()));
        }

        public static IEnumerable<(T previous, T current)> WithPrevious<T>(this IEnumerable<T> items)
        {
            T previous = default;

            foreach (var item in items)
            {
                yield return (previous, item);

                previous = item;
            }
        }

        public static IEnumerable<(T current, int index)> Indexed<T>(this IEnumerable<T> items)
        {
            int i = 0;

            foreach (var item in items)
            {
                yield return (item, i);

                i++;
            }
        }

        public static IEnumerable<(T current, bool isLast)> WithLastIdentification<T>(this IEnumerable<T> items)
        {
            for (int i = 0; i < items.Count(); i++)
            {
                yield return (items.ElementAt(i), i == items.Count() - 1);
            }
        }

        public static IEnumerable<(T item, bool isFirst, bool isLast)> WithFullIdentification<T>(this IEnumerable<T> items)
        {
            for (int i = 0; i < items.Count(); i++)
            {
                yield return (items.ElementAt(i), i == 0, i == items.Count() - 1);
            }
        }

        public static T GetNextOrFirst<T>(this IEnumerable<T> items, T item)
        {
            if (item == null)
            {
                return items.First();
            }

            return items.GetNextOrDefault(item) ?? items.First();
        }

        public static T GetNextOrDefault<T>(this IEnumerable<T> items, T item)
        {
            bool takeThis = false;
            foreach (var current in items)
            {
                if (takeThis)
                {
                    return current;
                }

                if (ReferenceEquals(current, item))
                {
                    takeThis = true;
                }
            }

            return default;
        }

        public static T GetNextOrDefault<T>(this IEnumerable<T> items, Func<T, bool> condition)
        {
            bool takeThis = false;
            foreach (var current in items)
            {
                if (takeThis)
                {
                    return current;
                }

                if (condition(current))
                {
                    takeThis = true;
                }
            }

            return default;
        }

        public static T GetPrevOrDefault<T>(this IEnumerable<T> items, T item)
        {
            if (items.Count() <= 1)
            {
                return default;
            }

            T previous = items.First();
            foreach (var current in items.Skip(1))
            {
                if (ReferenceEquals(current, item))
                {
                    return previous;
                }

                previous = current;
            }

            return default;
        }

        public static T GetPrevOrDefault<T>(this IEnumerable<T> items, Func<T, bool> condition)
        {
            if (items.Count() <= 1)
            {
                return default;
            }

            T previous = items.First();
            foreach (var current in items.Skip(1))
            {
                if (condition(current))
                {
                    return previous;
                }

                previous = current;
            }

            return default;
        }

        public static List<T> And<T>(this IEnumerable<T> items, T item)
        {
            var list = new List<T>(items);
            list.Add(item);
            return list;
        }

        public static int? IndexOfOrDefault<T>(this IEnumerable<T> items, Func<T, bool> condition)
        {
            foreach (var (current, index) in items.Indexed())
            {
                if (condition(current))
                {
                    return index;
                }
            }

            return null;
        }

        public static IEnumerable<string> NullSafe(this IEnumerable<string> items)
        {
            foreach (var item in items)
            {
                yield return item ?? String.Empty;
			}
        }
		
        public static IEnumerable<T> Where<T>(this IEnumerable items)
        {
            foreach (var item in items)
            {
                if (item is T t)
                {
                    yield return t;
                }
            }
        }

        public static string Dump(this IEnumerable<byte> arr)
        {
            return "[" + arr.Select(x => x.ToString()).JoinStringsWithoutSkipping(", ") + "]";
        }

        public static string Dump(this IEnumerable<int> arr)
        {
            return "[" + arr.Select(x => x.ToString()).JoinStringsWithoutSkipping(", ") + "]";
        }

        public static string Dump(this IEnumerable<float> arr)
        {
            return "[" + arr.Select(x => x.ToString("F")).JoinStringsWithoutSkipping(", ") + "]";
        }

        public static string Dump(this IEnumerable<bool> arr)
        {
            return "[" + arr.Select(x => x.ToString()).JoinStringsWithoutSkipping(", ") + "]";
        }

        public static string ToFormattedString<T>(this IEnumerable<T> arr, Func<T, string> toString = null)
        {
            toString = toString ?? ((x) => x.ToString());

            return "[" + arr.Select(x => toString(x)).JoinStringsWithoutSkipping(", ") + "]";
        }

        public static IEnumerable<T> InReverse<T>(this IEnumerable<T> origin)
        {
            var i = origin.ToList();
            i.Reverse();
            return i;
        }

        public static T Second<T>(this IEnumerable<T> origin) => origin.ElementAt(1);
        
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }

        public static Dictionary<TValue, TKey> InvertKeysAndValues<TKey, TValue>(this Dictionary<TKey, TValue> source)
        {
            var result = new Dictionary<TValue, TKey>();
            foreach (var value in source)
            {
                result.Add(value.Value, value.Key);
            }

            return result;
        }

        public static float AverageOrDefault(this IEnumerable<float> items, float d = 0)
        {
            if (items.Any())
            {
                return items.Average();
            }

            return d;
        }

        public static IEnumerable<T> Compress<T>(this IEnumerable<T> items)
        {
            var isFirst = true;
            var previous = default(T);
            foreach (var item in items)
            {
                if (isFirst)
                {
                    yield return item;
                    isFirst = false;
                    previous = item;
                }
                else
                {
                    if (item.Equals(previous) == false)
                    {
                        yield return item;
                        previous = item;
                    }
                }
            }
        }
    }
}