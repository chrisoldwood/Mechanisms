using Mechanisms.Extensions;
using Mechanisms.Tests;

namespace Tests.Extensions
{
    public static class StringExtensionsTests
    {
        public static void Define()
        {
            Suite.Add("A string is empty when its length is 0", () =>
            {
                const string emptyString = "";

                Assert.True(emptyString.IsEmpty());
            });

            Suite.Add("A string is not empty when its length is not 0", () =>
            {
                const string nonEmptyString = "not empty";

                Assert.False(nonEmptyString.IsEmpty());
            });

            Suite.Add("Querying a null reference for emptiness throws an exception", () =>
            {
                const string nullReference = null;

                Assert.Throws(() => nullReference.IsEmpty());
            });
        }
    }
}
