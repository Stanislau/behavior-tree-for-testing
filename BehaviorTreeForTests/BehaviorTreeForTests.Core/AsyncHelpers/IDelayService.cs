using System;
using System.Threading.Tasks;

namespace Medbullets.CrossCutting.AsyncHelpers
{
    public interface IDelayService
    {
        Task DelayAsync(TimeSpan time);
    }
}