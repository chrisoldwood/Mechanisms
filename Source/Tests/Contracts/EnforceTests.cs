using Mechanisms.Contracts;
using Mechanisms.Tests;

// Deliberating testing null references.
// ReSharper disable ExpressionIsAlwaysNull

namespace Tests.Contracts
{
    public static class EnforceTests
    {
        [TestCases]
        public static void true_validation()
        {
            Suite.Add("Enforcing true expression is benign when the result is true", () =>
            { 
                const bool trueExpression = true;

                Enforce.True(trueExpression, "must be true");

                Assert.Pass();
            });

            Suite.Add("Enforcing true expression throws when the result is false", () =>
            { 
                const bool falseExpression = false;

                Assert.Throws(() => Enforce.True(falseExpression, "must be true"));
            });
        }

        [TestCases]
        public static void false_validation()
        {
            Suite.Add("Enforcing false expression is benign when the result is false", () =>
            { 
                const bool falseExpression = false;

                Enforce.False(falseExpression, "must be false");

                Assert.Pass();
            });

            Suite.Add("Enforcing false expression throws when the result is true", () =>
            { 
                const bool trueExpression = true;

                Assert.Throws(() => Enforce.False(trueExpression, "must be false"));
            });
        }

        [TestCases]
        public static void null_validation()
        {
            Suite.Add("Enforcing non-null reference is benign when the reference is not null", () =>
            { 
                var @object = new object();

                Enforce.NotNull(@object, "@object");

                Assert.Pass();
            });

            Suite.Add("Enforcing non-null reference throws when the reference is null", () =>
            { 
                object nullReference = null;

                Assert.Throws(() => Enforce.NotNull(nullReference, "nullReference"));
            });
        }

        [TestCases]
        public static void string_validation()
        {
            Suite.Add("Enforcing non-empty string is benign when the string contains data", () =>
            { 
                const string nonEmptyString = "a string value";

                Enforce.NotEmpty(nonEmptyString, "nonEmptyString");

                Assert.Pass();
            });

            Suite.Add("Enforcing non-empty string throws when the string is null", () =>
            { 
                string nullReference = null;

                Assert.Throws(() => Enforce.NotEmpty(nullReference, "nullReference"));
            });

            Suite.Add("Enforcing non-empty string throws when the non-null string contains no data", () =>
            { 
                string emptyString = "";

                Assert.Throws(() => Enforce.NotEmpty(emptyString, "emptyString"));
            });
        }
    }
}
