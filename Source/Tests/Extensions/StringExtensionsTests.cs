using Mechanisms.Extensions;
using Mechanisms.Tests;

namespace Tests.Extensions
{
    public static class StringExtensionsTests
    {
        [TestCases]
        public static void emptiness()
        {
            "A string is empty when its length is 0".Is(() =>
            {
                const string emptyString = "";

                Assert.True(emptyString.IsEmpty());
            });

            "A string is not empty when its length is not 0".Is(() =>
            {
                const string nonEmptyString = "not empty";

                Assert.False(nonEmptyString.IsEmpty());
            });

            "Querying a null reference for emptiness throws an exception".Is(() =>
            {
                const string nullReference = null;

                Assert.Throws(() => nullReference.IsEmpty());
            });
        }

        [TestCases]
        public static void formatting()
        {
            "a string value can be used as a format string".Is(() =>
            {
                var output = "{0}".Fmt("value");

                Assert.True(output == "value");
            });
        }
    }
}
