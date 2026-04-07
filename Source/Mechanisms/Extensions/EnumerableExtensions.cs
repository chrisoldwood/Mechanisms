using System.Collections.Generic;

namespace Mechanisms.Extensions
{
    public static class EnumerableExtensions
    {
#if !NETSTANDARD2_0_OR_GREATER
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> sequence)
        {
            return new HashSet<T>(sequence);
        }
#endif
    }
}
