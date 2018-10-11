using System;
using Mechanisms.Tests;
using Mechanisms.Types;

// Explictly testing null reference scenarios.
// ReSharper disable ExpressionIsAlwaysNull
// Using "this" as a synonmym for lhs.
// ReSharper disable RedundantThisQualifier

namespace Tests.Types
{
    public static class OptionalTests
    {
        [TestCases]
        public static void construction()
        {
            "By default an optional has no value".Is(() =>
            {
                var optional = new Optional<string>();

                Assert.False(optional.HasValue);
            });

            "An optional has a value when constructed with one".Is(() =>
            {
                var optional = new Optional<string>("any value");

                Assert.True(optional.HasValue);
            });

            "An optional has no value when constructed with a null value reference".Is(() =>
            {
                var optional = new Optional<string>(null);

                Assert.False(optional.HasValue);
            });

            "An explicit shareable value exists for use as an empty value".Is(() =>
            {
                var none = Optional<string>.None;

                Assert.False(none.HasValue);
            });

            "An extension allows construction directly from a value".Is(() =>
            {
                var optional = "any value".ToOptional();

                Assert.True(optional.HasValue);
            });

            "An extension allows construction directly from a null value".Is(() =>
            {
                const string nullValue = null;

                var optional = nullValue.ToOptional();

                Assert.False(optional.HasValue);
            });
        }

        [TestCases]
        public static void comparison()
        {
            "An optional value is never equal to a null object".Is(() =>
            {
                var lhs = new Optional<IntValue>();
                object rhs = null;

                Assert.False(lhs.Equals(rhs));
            });

            "An optional value is never equal to an object of a different type".Is(() =>
            {
                var lhs = new Optional<IntValue>(new IntValue(42));
                object rhs = 42;

                Assert.False(lhs.Equals(rhs));
            });

            "An empty value is treated as equivalent to any other empty value".Is(() =>
            {
                var lhs = new Optional<IntValue>();
                var rhs = new Optional<IntValue>();

                Assert.True(lhs.Equals(rhs));
                Assert.True(lhs == rhs);
                Assert.False(lhs != rhs);
            });

            "An empty value is never treated as equivalent to a non-empty value".Is(() =>
            {
                var lhs = new Optional<IntValue>();
                var rhs = new Optional<IntValue>(new IntValue(42));

                Assert.False(lhs.Equals(rhs));
                Assert.False(rhs.Equals(lhs));
                Assert.False(lhs == rhs);
                Assert.False(rhs == lhs);
                Assert.True(lhs != rhs);
                Assert.True(rhs != lhs);
            });

            "Two of the same non-empty values will be treated as equivalent".Is(() =>
            {
                var lhs = new Optional<IntValue>(new IntValue(42));
                var rhs = new Optional<IntValue>(new IntValue(42));

                Assert.True(lhs.Equals(rhs));
                Assert.True(lhs == rhs);
                Assert.False(lhs != rhs);
            });

            "Two non-empty different values will not be treated as equivalent".Is(() =>
            {
                var lhs = new Optional<IntValue>(new IntValue(42));
                var rhs = new Optional<IntValue>(new IntValue(99));

                Assert.False(lhs.Equals(rhs));
                Assert.False(lhs == rhs);
                Assert.True(lhs != rhs);
            });
        }

        [TestCases]
        public static void hash_codes()
        {
            "The hash code for an empty value is zero".Is(() =>
            {
                var none = Optional<string>.None;

                Assert.True(none.GetHashCode() == 0);
            });

            "The hash code for an optional value is the same as the underlying value's hash code".Is(() =>
            {
                const int hashCode = 42;

                var value = new IntValue(hashCode);

                var some = new Optional<IntValue>(value);

                Assert.True(some.GetHashCode() == hashCode);
            });
        }

        [TestCases]
        public static void conversions()
        {
            "A value is implicitly convertable to an optional value".Is(() =>
            {
                const string value = "a value";

                Optional<string> optional = value;

                Assert.True(optional.HasValue);
            });

            "A null reference is implicitly convertable to an empty optional value".Is(() =>
            {
                const string nullValue = null;

                Optional<string> optional = nullValue;

                Assert.False(optional.HasValue);
            });
        }

        [TestCases]
        public static void unwrapping()
        {
            "The underlying value for an optional can be retrieved when not empty".Is(() =>
            {
                const string value = "a value";

                var optional = value.ToOptional();

                Assert.True(optional.Value == value);
            });

            "Querying for the underlying value of an empty optional throws an exception".Is(() =>
            {
                // Need assignment to force Value property to be read.
                #pragma warning disable 168

                var none = Optional<string>.None;

                Assert.Throws(() => { var x = none.Value; });

                #pragma warning restore 168
            });

            "the real value instead of the default can be retrieved when it exists".Is(() =>
            {
                const string value = "a value";
                const string @default = "the default";

                var optional = value.ToOptional();

                Assert.True(optional.ValueOrElse(@default) == value);
            });

            "the default value is returned when no value exists".Is(() =>
            {
                const string @default = "the default";

                var optional = Optional<string>.None;

                Assert.True(optional.ValueOrElse(@default) == @default);
            });

            "the real value is returned instead of invoking the default when it exists".Is(() =>
            {
                const string value = "a value";

                var defaultInvoked = false;
                Func<string> @default = () =>
                {
                    defaultInvoked = true;
                    return "the default";
                };

                var optional = value.ToOptional();

                Assert.True(optional.ValueOrElse(@default) == value);
                Assert.False(defaultInvoked);
            });

            "the default value generator is invoked when no value exists".Is(() =>
            {
                var defaultInvoked = false;
                Func<string> @default = () =>
                {
                    defaultInvoked = true;
                    return "the default";
                };

                var optional = Optional<string>.None;

                Assert.True(optional.ValueOrElse(@default) == "the default");
                Assert.True(defaultInvoked);
            });
        }

        private class IntValue
        {
            private int Value { get; set; }

            public IntValue(int value)
            {
                Value = value;
            }

            public override bool Equals(object rhs)
            {
                return this.Value == ((IntValue)rhs).Value;
            }

            public override int GetHashCode()
            {
                return Value;
            }
        }
    }
}
