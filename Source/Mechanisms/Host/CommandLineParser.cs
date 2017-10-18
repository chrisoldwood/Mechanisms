using System.Collections.Generic;
using System.Linq;
using Mechanisms.Extensions;

namespace Mechanisms.Host
{
    using ArgumentList = IList<string>;
    using ArgumentMap = IDictionary<int, IList<string>>;

    public static class CommandLineParser
    {
        public static Arguments Parse(IEnumerable<string> arguments, IEnumerable<Switch> switches)
        {
            var argList = arguments.ToArray();
            var switchList = switches.ToArray();

            ValidateSwitches(switchList);

            var named = new Dictionary<int, ArgumentList>();
            var unnamed = new List<string>();
            for (var i = 0; i != argList.Length; ++i)
            {
                var argument = argList[i];
                var values = new List<string>();

                if (argument.StartsWith("-") || argument.StartsWith("/"))
                {
                    var longName = argument.StartsWith("--");
                    var prefixLen = longName ? 2 : 1;
                    var valuePos = argument.IndexOfAny(new[]{ ':', '=' });
                    var name = (valuePos == -1) ? argument.Substring(prefixLen)
                                                : argument.Substring(prefixLen, valuePos - prefixLen);

                    var @switch = longName ? switchList.Single(s => s.LongName == name)
                                           : switchList.Single(s => s.ShortName == name);

                    if ((@switch.Type == Switch.ValueType.Boolean) && (valuePos != -1))
                    {
                        throw new CmdLineException("The switch '{0}' does not expect a value".Fmt(name));
                    }
                    else if (@switch.Type == Switch.ValueType.Value)
                    {
                        var value = (valuePos == -1) ? argList[++i]
                                                     : argument.Substring(valuePos+1);
                        values.Add(value);
                    }

                    named.Add(@switch.Id, values);
                }
                else
                {
                    unnamed.Add(argument);
                }
            }

            return new Arguments(named, unnamed);
        }

        private static void ValidateSwitches(IEnumerable<Switch> switches)
        {
            var shortNames = new HashSet<string>();
            var longNames = new HashSet<string>();

            foreach (var @switch in switches)
            {
                if (@switch.ShortName.HasValue)
                {
                    var shortName = @switch.ShortName.Value;

                    if (shortName.IsEmpty())
                        throw new InvalidSwitchException("Empty short name for switch ID: {0}".Fmt(@switch.Id));

                    if (shortNames.Contains(shortName))
                        throw new InvalidSwitchException("Duplicate short name: {0} for switch ID: {1}".Fmt(@switch.ShortName, @switch.Id));

                    shortNames.Add(shortName);
                }

                if (@switch.LongName.HasValue)
                {
                    var longName = @switch.LongName.Value;

                    if (longName.IsEmpty())
                        throw new InvalidSwitchException("Empty long name for switch ID: {0}".Fmt(@switch.Id));

                    if (longNames.Contains(longName))
                        throw new InvalidSwitchException("Duplicate long name: {0} for switch ID: {1}".Fmt(@switch.LongName, @switch.Id));

                    longNames.Add(longName);
                }
            }
        }
    }
}
