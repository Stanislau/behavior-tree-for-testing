using System;
using System.Threading.Tasks;

namespace Medbullets.CrossCutting.AsyncHelpers
{
    public class TaskDelayService : IDelayService
    {
        public Task DelayAsync(TimeSpan time)
        {
            return Task.Delay(time);
        }
    }
}