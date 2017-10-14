using Mechanisms.Types;

namespace Mechanisms.Host
{
    public class Switch
    {
        public int Id { get; private set; }
        public Optional<string> ShortName { get; private set; }
        public Optional<string> LongName { get; private set; }

        public static readonly Optional<string> NoShortName = Optional<string>.None;
        public static readonly Optional<string> NoLongName = Optional<string>.None;

        public Switch(int id, Optional<string> shortName, Optional<string> longName)
        {
            Id = id;
            ShortName = shortName;
            LongName = longName;
        }
    }
}