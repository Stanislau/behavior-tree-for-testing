using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Medbullets.CrossCutting.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum GenericEnum)
        {
            Type genericEnumType = GenericEnum.GetType();
            MemberInfo[] memberInfo = genericEnumType.GetMember(GenericEnum.ToString());
            if ((memberInfo != null && memberInfo.Length > 0))
            {
                var atts = memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if ((atts != null && atts.Count() > 0))
                {
                    return ((System.ComponentModel.DescriptionAttribute)atts.ElementAt(0)).Description;
                }
            }
            return GenericEnum.ToString();
        }

        public static IEnumerable<Enum> GetUniqueFlags(this Enum flags)
        {
            var flag = 1ul;
            foreach (var value in Enum.GetValues(flags.GetType()).Cast<Enum>())
            {
                var bits = Convert.ToUInt64(value);
                while (flag < bits)
                {
                    flag <<= 1;
                }

                if (flag == bits && flags.HasFlag(value))
                {
                    yield return value;
                }
            }
        }

        public static IEnumerable<T> SelfOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable ?? Enumerable.Empty<T>();
        }

        public static short[] ToShortArray<T>() where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("Value must be an enum");

            var array = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
            var customArray = Array.ConvertAll(array, value => Convert.ToInt16(value));

            return customArray;
        }

        public static bool IsOneOf<T>(this T flag, params T[] items) where T : Enum
        {
            return items.Contains(flag);
        }
    }
}
