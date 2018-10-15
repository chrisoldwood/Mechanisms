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

        public CommandLineGrammar Grammar { get { return _grammar; } }

        public CommandLineParser(IEnumerable<string> arguments, IEnumerable<Switch> switches)
            : this(arguments, new CommandLineGrammar(switches))
        {
        }

        public CommandLineParser(IEnumerable<string> arguments, CommandLineGrammar grammar)
        {
            _arguments = arguments.ToArray();
            _grammar = grammar;
        }

        public Arguments Parse()
        {
            return Parse(_arguments, _grammar.Switches);
        }

        public static Arguments Parse(IEnumerable<string> arguments, IEnumerable<Switch> switches)
        {
            var argList = arguments.ToArray();
            var switchList = switches.ToArray();

            CommandLineGrammar.ValidateSwitches(switchList);

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

        private readonly string[] _arguments;
        private readonly CommandLineGrammar _grammar;
    }
}
