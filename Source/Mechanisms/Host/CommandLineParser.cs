using System;
using System.Collections.Generic;
using System.Linq;
using Mechanisms.Contracts;
using Mechanisms.Extensions;

namespace Mechanisms.Host
{
    using ArgumentList = IList<string>;
    using ArgumentMap = IDictionary<int, IList<string>>;

    public class CommandLineParser
    {
        public const int HelpSwitch = -1;
        public const int VersionSwitch = -2;
        public const int FirstCustomSwitch = 1;

        public CommandLineParser(IEnumerable<string> arguments, IEnumerable<Switch> switches)
        {
            _arguments = arguments.ToArray();
            _switches = switches.ToArray();
        }

        public Arguments Parse()
        {
            return Parse(_arguments, _switches);
        }

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

                    if (String.IsNullOrEmpty(name))
                    {
                        throw new CmdLineException("The switch name for argument '{0}' was missing".Fmt(argument));
                    }

                    var @switch = longName ? switchList.SingleOrDefault(s => s.LongName == name)
                                           : switchList.SingleOrDefault(s => s.ShortName == name);

                    if (@switch == null)
                    {
                        throw new CmdLineException("The switch '{0}' is not recognised".Fmt(name));
                    }
                    else if ((@switch.Type == Switch.ValueType.Boolean) && (valuePos != -1))
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

        public IEnumerable<string> FormatSwitches()
        {
            return FormatSwitches(_switches);
        }

        public static IEnumerable<string> FormatSwitches(IEnumerable<Switch> switches)
        {
            var switchList = switches.ToArray();

            int maxShortNameLen = 0;
            int maxLongNameLen = 0;

            foreach (var @switch in switchList)
            {
                if (@switch.ShortName.HasValue)
                    maxShortNameLen = Math.Max(maxShortNameLen, @switch.ShortName.Value.Length);

                if (@switch.LongName.HasValue)
                    maxLongNameLen = Math.Max(maxLongNameLen, @switch.LongName.Value.Length);
            }

            var usage = new List<string>();

            foreach (var @switch in switchList)
            {
                var shortName = @switch.ShortName.HasValue ? "-" + @switch.ShortName.Value : "";
                var longName = @switch.LongName.HasValue ? "--" + @switch.LongName.Value : "";
                var description = @switch.Description;
                var line = "";

                if (@switch.ShortName.HasValue && @switch.LongName.HasValue)
                {
                    var lftPadLen = maxShortNameLen - @switch.ShortName.Value.Length; 
                    var lftPadding = new string(' ', lftPadLen);
                    var rgtPadLen = maxLongNameLen - @switch.LongName.Value.Length; 
                    var rgtPadding = new string(' ', rgtPadLen);

                    line = "{0}{1} | {2}{3}  {4}".Fmt(shortName, lftPadding, longName, rgtPadding, description);
                }
                else if (@switch.ShortName.HasValue)
                {
                    var lftPadLen = maxShortNameLen - @switch.ShortName.Value.Length; 
                    var lftPadding = new string(' ', lftPadLen);
                    var rgtPadLen = maxLongNameLen != 0 ? (3 + 2 + maxLongNameLen) : 0; 
                    var rgtPadding = new string(' ', rgtPadLen);

                    line = "{0}{1}{2}  {3}".Fmt(shortName, lftPadding, rgtPadding, description);
                }
                else if (@switch.LongName.HasValue)
                {
                    var lftPadLen = maxShortNameLen != 0 ? (1 + maxShortNameLen + 3) : 0; 
                    var lftPadding = new string(' ', lftPadLen);
                    var rgtPadLen = maxLongNameLen - @switch.LongName.Value.Length; 
                    var rgtPadding = new string(' ', rgtPadLen);

                    line = "{0}{1}{2}  {3}".Fmt(lftPadding, longName, rgtPadding, description);
                }
                else
                {
                    throw new ContractViolationException("No short or long name defined");
                }

                usage.Add(line);
            }

            return usage;
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

        private readonly string[] _arguments;
        private readonly Switch[] _switches;
    }
}
