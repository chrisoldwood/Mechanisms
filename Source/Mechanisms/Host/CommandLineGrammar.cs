using System;
using System.Collections.Generic;
using System.Linq;
using Mechanisms.Contracts;
using Mechanisms.Extensions;

namespace Mechanisms.Host
{
    public class CommandLineGrammar
    {
        public IEnumerable<Switch> Switches { get { return _switches; } }

        public CommandLineGrammar(IEnumerable<Switch> switches)
        {
            _switches = switches.ToArray();
        }

        public IEnumerable<string> FormatSwitches()
        {
            return FormatSwitches(_switches);
        }

        public void ValidateSwitches()
        {
            ValidateSwitches(_switches);
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

        public static void ValidateSwitches(IEnumerable<Switch> switches)
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

        private readonly Switch[] _switches;
    }
}
