using System.Collections.Generic;
using System.Linq;
using Mechanisms.Extensions;
using Mechanisms.Tests;

namespace Tests.Extensions
{
    public static class EnumerableExtensionsTests
    {
        [TestCases]
        public static void conversions()
        {
            "A sequence can be converted to a hash set".Is(() =>
            {
                var seq = new int[] {1, 2, 3};

                var set = seq.ToHashSet();

                Assert.True(set.Count == seq.Length);
                Assert.True(HashSetContainsSequence(set, seq));
            });
        }

        private static bool HashSetContainsSequence(HashSet<int> set, IEnumerable<int> sequence)
        {
            return sequence.All(value => set.Contains(value));
        }
    }
}
