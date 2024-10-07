using System;
using System.Threading.Tasks;

namespace Medbullets.CrossCutting.AsyncHelpers
{
    public static class DelayServiceExtensions
    {
        public static Task Delay(this TimeSpan timeSpan, IDelayService delayService) => delayService.DelayAsync(timeSpan);
    }
}