using System;
using System.Threading;

namespace Medbullets.CrossCutting.Extensions
{
    public static class ExceptionExtensions
    {
        public static bool CausedBy(this Exception exception, CancellationToken ct)
        {
            var e = exception as OperationCanceledException;
            return e != null && e.CancellationToken == ct;
        }
    }
}