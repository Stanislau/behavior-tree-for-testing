using System;
using System.Collections.Generic;

namespace Medbullets.CrossCutting.Extensions
{
    public static class ObjectExtensions
    {
        public static T CastTo<T>(this object obj) => (T) obj;

        public static bool EqualsGeneric<T>(this object self, object obj, Func<T, T, bool> equalsTo)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(self, obj)) return true;
            if (obj.GetType() != self.GetType()) return false;
            return equalsTo((T)self, (T) obj);
        }

        public static T Make<T>(this T self, Action<T> action)
        {
            action(self);

            return self;
        }

        public static List<T> DeepClone<T>(this List<T> self, Func<T, T> clone)
        {
            var copy = new List<T>();

            foreach (var item in self)
            {
                copy.Add(clone(item));
            }

            return copy;
        }

        public static bool TryAssignTo<T>(this T self, ref T source)
        {
            if (source.Equals(self) == false)
            {
                source = self;

                return true;
            }

            return false;
        }

        public static T AddInto<T>(this T self, List<T> source)
        {
            source.Add(self);

            return self;
        }

        public static T As<T>(this T view, out T result)
        {
            result = view;

            return view;
        }

        public static float ToFloat(this int i) => i;

        public static float ToFloat(this byte i) => i;

        public static string ToFormattedString(this bool condition, string ifTrue, string ifFalse = null)
        {
            if (condition)
            {
                return ifTrue;
            }
            else
            {
                return ifFalse ?? string.Empty;
            }
        }
    }
}