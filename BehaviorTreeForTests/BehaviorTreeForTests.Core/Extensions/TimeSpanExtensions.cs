using System;

namespace Medbullets.CrossCutting.Extensions
{
    public static class TimeSpanExtensions
    {
        public static TimeSpan Milliseconds(this int ms) => TimeSpan.FromMilliseconds(ms);

        public static TimeSpan Seconds(this int seconds) => TimeSpan.FromSeconds(seconds);

        public static TimeSpan Minutes(this int minutes) => TimeSpan.FromMinutes(minutes);
    }
}