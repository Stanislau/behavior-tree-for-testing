using System.Collections.Generic;
using System.Text;

namespace Medbullets.CrossCutting.Extensions
{
    public static class StringExtensions
    {
        public static string JoinStringsWithoutSkipping(this IEnumerable<string> self, string separator)
        {
            var result = new StringBuilder();

            foreach (var str in self)
            {
                result.Append($"{str}{separator}");
            }

            if (result.Length < separator.Length)
            {
                return result.ToString();
            }

            return result.ToString(0, result.Length - separator.Length);
        }

        public static bool IsNullOrWhiteSpaceString(this string self) => string.IsNullOrWhiteSpace(self);

        public static bool IsNotEmpty(this string self) => self.IsNullOrWhiteSpaceString() == false;

        public static string FormatWith(this string self, params object[] args) => string.Format(self, args);

        public static bool EqualsAndConsiderNullAsEmpty(this string self, string another)
        {
            var left = self ?? string.Empty;
            var right = another ?? string.Empty;

            return left == right;
        }
    }
}