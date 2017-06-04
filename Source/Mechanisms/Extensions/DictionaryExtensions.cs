using System.Collections.Generic;
using Mechanisms.Types;

// ReSharper disable InconsistentNaming

namespace Mechanisms.Extensions
{
    public static class DictionaryExtensions
    {
        public static Optional<V> TryGetValue<K,V>(this IDictionary<K,V> dictionary, K key)
            where V : class
        {
            V value;

            if (!dictionary.TryGetValue(key, out value))
                return Optional<V>.None;

            return value;
        }
    }
}
