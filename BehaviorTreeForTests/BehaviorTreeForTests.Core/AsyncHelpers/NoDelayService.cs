using System;
using System.Threading.Tasks;

namespace Medbullets.CrossCutting.AsyncHelpers
{
    public class NoDelayService : IDelayService
    {
        public Task DelayAsync(TimeSpan time)
        {
            return Task.CompletedTask;
        }
    }
}