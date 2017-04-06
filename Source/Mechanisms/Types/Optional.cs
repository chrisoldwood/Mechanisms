
// Using "this" as a synonym for "lhs".
// ReSharper disable RedundantThisQualifier

using Mechanisms.Contracts;

namespace Mechanisms.Types
{
    public struct Optional<T>
        where T : class
    {
        public bool HasValue { get { return _value != null; } }

        public T Value
        {
            get
            {
                Expect.NotNull(_value, "value");

                return _value;
            }
        }

        public static readonly Optional<T> None = new Optional<T>();

        public Optional(T value)
        {
            _value = value;
        }

        public override bool Equals(object rhs)
        {
            if (rhs == null)
                return false;

            if (rhs.GetType() != this.GetType())
                return false;
            
            return Equals((Optional<T>)rhs);
        }

        public bool Equals(Optional<T> rhs)
        {
            return (this._value == null && rhs._value == null)
                || (this._value != null && rhs._value != null && this._value.Equals(rhs._value));
        }

        public static bool operator==(Optional<T> lhs, Optional<T> rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator!=(Optional<T> lhs, Optional<T> rhs)
        {
            return !lhs.Equals(rhs);
        }

        public override int GetHashCode()
        {
            return (_value != null) ? _value.GetHashCode() : 0;
        }

        public static implicit operator Optional<T>(T value) 
        {
            return new Optional<T>(value);
        } 

        private readonly T _value;
    }

    public static class OptionalExtensions
    {
        public static Optional<T> ToOptional<T>(this T value)
            where T : class
        {
            return new Optional<T>(value);
        }
    }
}
