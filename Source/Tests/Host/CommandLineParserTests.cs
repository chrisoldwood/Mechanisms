using System.Linq;
using Mechanisms.Host;
using Mechanisms.Tests;

namespace Tests.Host
{
    public static class CommandLineParserTests
    {
        [TestCases]
        public static void switch_validation()
        {
            "the same switch can have mutiple short and long names".Is(() =>
            {
                const int Help = 1;

                var switches = new[]
                {
                    new Switch(Help, "h", "help"),
                    new Switch(Help, "?", null),
                };

                CommandLineParser.Parse(EmptyArgList, switches);

                Assert.Pass();
            });

            "switch short names cannot be empty".Is(() =>
            {
                const int anyId = 99;

                var switches = new[]
                {
                    new Switch(anyId, "", Switch.NoLongName),
                };

                Assert.Throws(() => CommandLineParser.Parse(EmptyArgList, switches));
            });

            "switch short names must be unique".Is(() =>
            {
                const int anyId = 99;

                var switches = new[]
                {
                    new Switch(anyId, "s", Switch.NoLongName),
                    new Switch(anyId, "s", Switch.NoLongName),
                };

                Assert.Throws(() => CommandLineParser.Parse(EmptyArgList, switches));
            });

            "switch long names cannot be empty".Is(() =>
            {
                const int anyId = 99;

                var switches = new[]
                {
                    new Switch(anyId, null, ""),
                };

                Assert.Throws(() => CommandLineParser.Parse(EmptyArgList, switches));
            });

            "switch long names must be unique".Is(() =>
            {
                const int anyId = 99;

                var switches = new[]
                {
                    new Switch(anyId, Switch.NoShortName, "switch"),
                    new Switch(anyId, Switch.NoShortName, "switch"),
                };

                Assert.Throws(() => CommandLineParser.Parse(EmptyArgList, switches));
            });
        }

        [TestCases]
        public static void basic_parsing()
        {
            "an empty list contains no arguments".Is(() =>
            {
                var args = new string[0];

                var switches = new Switch[0];
                var result = CommandLineParser.Parse(args, switches);

                Assert.True(result.Unnamed.Count == 0);
                Assert.True(result.Named.Count == 0);
            });

            "arguments not bound to a switch are collected as unamed arguments".Is(() =>
            {
                var args = new[] { "unnnamed-argument" };

                var switches = new Switch[0];
                var result = CommandLineParser.Parse(args, switches);

                Assert.True(result.Unnamed.Count == 1);
                Assert.True(result.Named.Count == 0);
            });

            "named arguments can be matched on their long name".Is(() =>
            {
                var args = new[] { "--version" };

                var switches = new[] { VersionSwitch };
                var result = CommandLineParser.Parse(args, switches);

                Assert.True(result.Named.Count == 1);
                Assert.True(result.Named.ContainsKey(Version));
                Assert.True(result.Unnamed.Count == 0);
            });

            "named arguments can be matched on their short name".Is(() =>
            {
                var args = new[] { "-v" };

                var switches = new[] { VersionSwitch };
                var result = CommandLineParser.Parse(args, switches);

                Assert.True(result.Named.Count == 1);
                Assert.True(result.Named.ContainsKey(Version));
                Assert.True(result.Unnamed.Count == 0);
            });

            "short name switches can be prefixed with a '/'".Is(() =>
            {
                var args = new[] { "/v" };

                var switches = new[] { VersionSwitch };
                var result = CommandLineParser.Parse(args, switches);

                Assert.True(result.Named.Count == 1);
                Assert.True(result.Named.ContainsKey(Version));
                Assert.True(result.Unnamed.Count == 0);
            });
        }

        [TestCases]
        public static void boolean_value_parsing()
        {
            "a boolean switch requires no value".Is(() =>
            {
                var args = new[] { "--version" };

                var switches = new[] { VersionSwitch };
                var result = CommandLineParser.Parse(args, switches);

                Assert.True(result.IsSet(Version));
            });

            "specifying a value for a boolean switch generates an error".Is(() =>
            {
                var switches = new[] { VersionSwitch };
                var args = new[] { "--version:value" };

                Assert.Throws(() => CommandLineParser.Parse(args, switches));
            });
        }

        [TestCases]
        public static void single_value_parsing()
        {
            "the value for a single value switch can be specfied with the subsequent argument".Is(() =>
            {
                var switches = new[] { LoginSwitch };
                var scenarios = new[] 
                {
                    new[] { "-l", "value" },
                    new[] { "/l", "value" },
                    new[] { "--login", "value" },
                };

                foreach (var args in scenarios)
                {
                    var result = CommandLineParser.Parse(args, switches);

                    Assert.True(result.IsSet(Login));
                    Assert.True(result.Value(Login) == "value");
                }
            });

            "if the value for a single value switch is missing it generates an error".Is(() =>
            {
                var switches = new[] { LoginSwitch, VersionSwitch };
                var scenarios = new[] 
                {
                    new[] { "-l", },
                    new[] { "/l", },
                    new[] { "--login", },
                };

                foreach (var args in scenarios)
                {
                    Assert.Throws(() => CommandLineParser.Parse(args, switches));
                }
            });

            "the value for a single value switch can be specfied inline after a : or =".Is(() =>
            {
                var switches = new[] { LoginSwitch };
                var scenarios = new[] 
                {
                    new[] { "-l:value" },
                    new[] { "/l:value" },
                    new[] { "--login:value" },
                    new[] { "-l=value" },
                    new[] { "/l=value" },
                    new[] { "--login=value" },
                };

                foreach (var args in scenarios)
                {
                    var result = CommandLineParser.Parse(args, switches);

                    Assert.True(result.IsSet(Login));
                    Assert.True(result.Value(Login) == "value");
                }
            });
        
            "the value for a single value switch can be empty".Is(() =>
            {
                var switches = new[] { LoginSwitch };
                var args = new[] { "--login:" };

                var result = CommandLineParser.Parse(args, switches);

                Assert.True(result.IsSet(Login));
                Assert.True(result.Value(Login) == "");
            });
        }

        [TestCases]
        public static void multiple_switch_parsing()
        {
            "multiple switches can be passed in any order using any style".Is(() =>
            {
                var switches = new[] { VersionSwitch, LoginSwitch };
                var scenarios = new[] 
                {
                    new[] { "--login", "value", "--version" },
                    new[] { "--version", "--login", "value" },
                    new[] { "-l:value", "--version" },
                    new[] { "--login", "value", "/v" },
                };

                foreach (var args in scenarios)
                {
                    var result = CommandLineParser.Parse(args, switches);

                    Assert.True(result.IsSet(Version));
                    Assert.True(result.IsSet(Login));
                    Assert.True(result.Value(Login) == "value");
                }
            });

            "a switch will be interpreated as a value rather than a missing value being detected".Is(() =>
            {
                var switches = new[] { LoginSwitch, VersionSwitch };
                var args = new[] { "--login", "--version" };

                var result = CommandLineParser.Parse(args, switches);

                Assert.True(result.IsSet(Login));
                Assert.False(result.IsSet(Version));
                Assert.True(result.Value(Login) == "--version");
            });
        }

        [TestCases]
        public static void results_querying()
        {
            "simple boolean switches can be queried".Is(() =>
            {
                var args = new[] { "--version" };

                var switches = new[] { new Switch(Version, Switch.NoShortName, "version"), };
                var arguments = CommandLineParser.Parse(args, switches);

                Assert.True(arguments.IsSet(Version));
            });

            "querying if an unspecfied switch was present returns false".Is(() =>
            {
                const int Unspecified = 2;
                var UnspecifiedSwitch = new Switch(Unspecified, "u", "unspecified");

                var args = new[] { "other args" };

                var switches = new[] { UnspecifiedSwitch, };
                var arguments = CommandLineParser.Parse(args, switches);

                Assert.False(arguments.IsSet(Unspecified));
            });

            "unnamed arguments remain in order".Is(() =>
            {
                var args = new[] { "first", "--version", "second" };

                var switches = new[] { new Switch(Version, Switch.NoShortName, "version"), };
                var arguments = CommandLineParser.Parse(args, switches);

                Assert.True(arguments.Unnamed.SequenceEqual(new[] { "first", "second" }));
            });
        }

        private const int Version = 1;
        private const int Login = 2;

        private static readonly Switch VersionSwitch = new Switch(Version, "v", "version");
        private static readonly Switch LoginSwitch = new Switch(Login, "l", "login", Switch.ValueType.Value);
        private static readonly string[] EmptyArgList = new string[0];
    }
}
