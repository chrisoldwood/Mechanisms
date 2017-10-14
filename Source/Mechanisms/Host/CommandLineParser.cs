using System;
using System.Collections.Generic;
using System.Linq;
using Mechanisms.Extensions;

// ReSharper disable PossibleMultipleEnumeration

namespace Mechanisms.Host
{
    using ArgumentList = IList<string>;
    using ArgumentMap = IDictionary<int, IList<string>>;

    public static class CommandLineParser
    {
        public static Arguments Parse(IEnumerable<string> arguments, IEnumerable<Switch> switches)
        {
            ValidateSwitches(switches);

            var named = new Dictionary<int, ArgumentList>();
            var unnamed = new List<string>();

            foreach (var argument in arguments)
            {
                if (argument.StartsWith("--"))
                {
                    var name = argument.Substring(2);
                    var @switch = switches.Single(s => s.LongName == name);
                    named.Add(@switch.Id, new List<string>());
                }
                else if (argument.StartsWith("-") || argument.StartsWith("/"))
                {
                    var name = argument.Substring(1);
                    var @switch = switches.Single(s => s.ShortName == name);
                    named.Add(@switch.Id, new List<string>());
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
