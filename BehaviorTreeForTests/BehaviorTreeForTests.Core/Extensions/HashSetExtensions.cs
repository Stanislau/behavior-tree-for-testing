using System.Collections.Generic;

namespace Medbullets.CrossCutting.Extensions
{
    public static class HashSetExtensions
    {
        public static bool Toggle<T>(this HashSet<T> set, T value)
        {
            if (set.Contains(value))
            {
                set.Remove(value);
                return false;
            }
            else
            {
                set.Add(value);
                return true;
            }
        }
    }
}