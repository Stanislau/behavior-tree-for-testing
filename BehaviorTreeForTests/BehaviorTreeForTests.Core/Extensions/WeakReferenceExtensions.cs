using System;

namespace Medbullets.CrossCutting.Extensions
{
    public static class WeakReferenceExtensions
    {
        public static T GetTargetOrDefault<T>(this WeakReference<T> reference) where T : class
        {
            if (reference == null)
            {
                return null;
            }

            T target;
            return reference.TryGetTarget(out target) ? target : default(T);
        }
    }
}