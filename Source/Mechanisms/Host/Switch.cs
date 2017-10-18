using Mechanisms.Types;

namespace Mechanisms.Host
{
    public class Switch
    {
        public enum ValueType
        {
            Boolean = 1,
            Value,
        }

        public int Id { get; private set; }
        public Optional<string> ShortName { get; private set; }
        public Optional<string> LongName { get; private set; }
        public ValueType Type { get; private set; }

        public static readonly Optional<string> NoShortName = Optional<string>.None;
        public static readonly Optional<string> NoLongName = Optional<string>.None;

        public Switch(int id, Optional<string> shortName, Optional<string> longName)
            : this(id, shortName, longName, ValueType.Boolean)
        {
        }

        public Switch(int id, Optional<string> shortName, Optional<string> longName, ValueType type)
        {
            Id = id;
            ShortName = shortName;
            LongName = longName;
            Type = type;
        }
    }
}