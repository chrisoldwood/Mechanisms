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
            "Expecting a true expression is benign when the result is true".Is(() =>
            { 
                const bool trueExpression = true;

                Expect.True(trueExpression, "must be true");

                Assert.Pass();
            });

            "Expecting a true expression throws when the result is false".Is(() =>
            { 
                const bool falseExpression = false;

                Assert.Throws(() => Expect.True(falseExpression, "must be true"));
            });
        }

        [TestCases]
        public static void false_validation()
        {
            "Expecting a false expression is benign when the result is false".Is(() =>
            { 
                const bool falseExpression = false;

                Expect.False(falseExpression, "must be false");

                Assert.Pass();
            });

            "Expecting a false expression throws when the result is true".Is(() =>
            { 
                const bool trueExpression = true;

                Assert.Throws(() => Expect.False(trueExpression, "must be false"));
            });
        }

        [TestCases]
        public static void null_validation()
        {
            "Expecting a non-null reference is benign when the reference is not null".Is(() =>
            { 
                var @object = new object();

                Expect.NotNull(@object, "@object");

                Assert.Pass();
            });

            "Expecting a non-null reference throws when the reference is null".Is(() =>
            { 
                object nullReference = null;

                Assert.Throws(() => Expect.NotNull(nullReference, "nullReference"));
            });
        }

        [TestCases]
        public static void string_validation()
        {
            "Expecting a non-empty string is benign when the string contains data".Is(() =>
            { 
                const string nonEmptyString = "a string value";

                Expect.NotEmpty(nonEmptyString, "nonEmptyString");

                Assert.Pass();
            });

            "Expecting a non-empty string throws when the string is null".Is(() =>
            { 
                string nullReference = null;

                Assert.Throws(() => Expect.NotEmpty(nullReference, "nullReference"));
            });

            "Expecting a non-empty string throws when the non-null string contains no data".Is(() =>
            { 
                const string emptyString = "";

                Assert.Throws(() => Expect.NotEmpty(emptyString, "emptyString"));
            });
        }
    }
}
