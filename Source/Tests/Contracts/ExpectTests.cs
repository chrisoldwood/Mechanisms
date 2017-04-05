using Mechanisms.Contracts;
using Mechanisms.Tests;

// Deliberating testing null references.
// ReSharper disable ExpressionIsAlwaysNull
// ReSharper disable AssignNullToNotNullAttribute

namespace Tests.Contracts
{
    public static class ExpectTests
    {
        [TestCases]
        public static void true_validation()
        {
            Suite.Add("Expecting a true expression is benign when the result is true", () =>
            { 
                const bool trueExpression = true;

                Expect.True(trueExpression, "must be true");

                Assert.Pass();
            });

            Suite.Add("Expecting a true expression throws when the result is false", () =>
            { 
                const bool falseExpression = false;

                Assert.Throws(() => Expect.True(falseExpression, "must be true"));
            });
        }

        [TestCases]
        public static void false_validation()
        {
            Suite.Add("Expecting a false expression is benign when the result is false", () =>
            { 
                const bool falseExpression = false;

                Expect.False(falseExpression, "must be false");

                Assert.Pass();
            });

            Suite.Add("Expecting a false expression throws when the result is true", () =>
            { 
                const bool trueExpression = true;

                Assert.Throws(() => Expect.False(trueExpression, "must be false"));
            });
        }

        [TestCases]
        public static void null_validation()
        {
            Suite.Add("Expecting a non-null reference is benign when the reference is not null", () =>
            { 
                var @object = new object();

                Expect.NotNull(@object, "@object");

                Assert.Pass();
            });

            Suite.Add("Expecting a non-null reference throws when the reference is null", () =>
            { 
                object nullReference = null;

                Assert.Throws(() => Expect.NotNull(nullReference, "nullReference"));
            });
        }

        [TestCases]
        public static void string_validation()
        {
            Suite.Add("Expecting a non-empty string is benign when the string contains data", () =>
            { 
                const string nonEmptyString = "a string value";

                Expect.NotEmpty(nonEmptyString, "nonEmptyString");

                Assert.Pass();
            });

            Suite.Add("Expecting a non-empty string throws when the string is null", () =>
            { 
                string nullReference = null;

                Assert.Throws(() => Expect.NotEmpty(nullReference, "nullReference"));
            });

            Suite.Add("Expecting a non-empty string throws when the non-null string contains no data", () =>
            { 
                const string emptyString = "";

                Assert.Throws(() => Expect.NotEmpty(emptyString, "emptyString"));
            });
        }
    }
}
