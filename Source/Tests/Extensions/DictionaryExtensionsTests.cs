using System.Collections.Generic;
using Mechanisms.Extensions;
using Mechanisms.Tests;

namespace Tests.Extensions
{
    public static class DictionaryExtensionsTests
    {
        [TestCases]
        public static void retrieval()
        {
            "trying to find a value returns no value when not found".Is(() =>
            {
                var dictionary = new Dictionary<string, string>();

                var result = dictionary.TryGetValue("unknown key");

                Assert.False(result.HasValue);
            });

            "trying to find a value returns it when the key is found".Is(() =>
            {
                var dictionary = new Dictionary<string, string>{ {"known key", "the value"} };

                var result = dictionary.TryGetValue("known key");

                Assert.True(result.HasValue);
                Assert.True(result.Value == "the value");
            });
        }
    }
}
