using System.Collections.Generic;

namespace Mechanisms.Extensions
{
    public static class EnumerableExtensions
    {
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> sequence)
        {
            return new HashSet<T>(sequence);
        }
    }
}
