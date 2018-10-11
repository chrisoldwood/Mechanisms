using System;
using System.Diagnostics;
using System.IO;
using Mechanisms.Host;
using Mechanisms.Tests;
using Switch = Mechanisms.Host.Switch;

// We want our lambdas to look like real methods.
// ReSharper disable ConvertToLambdaExpression
// ReSharper disable RedundantLambdaSignatureParentheses

namespace Tests.Host
{
    using ArgsMainFunc = Func<string[], int>;
    using ParserMainFunc = Func<Arguments, int>;

    public static class BootstrapperTests
    {
        [TestCases]
        public static void exit_codes()
        {
            "By default the boostrapper passes through the exit code from main()".Is(() =>
            {
                const int mainExitCode = 99;

                ArgsMainFunc main = (args) =>
                {
                    return mainExitCode;
                };

                var exitCode = Bootstrapper.Run(main, AnyArgs, AnyStdIn, AnyStdOut, AnyStdErr, AnyDebugWriter);

                Assert.True(exitCode == mainExitCode);
            });

            "The boostrapper returns a non-zero value when an exception is not caught".Is(() =>
            {
                ArgsMainFunc main = (args) =>
                {
                    throw new Exception("any exception");
                };

                var exitCode = Bootstrapper.Run(main, AnyArgs, AnyStdIn, AnyStdOut, AnyStdErr, AnyDebugWriter);

                Assert.True(exitCode != 0);
            });
        }

        [TestCases]
        public static void error_reporting()
        {
            "The message for an uncaught exception is written to stderr".Is(() =>
            {
                const string message = "an exception message";

                ArgsMainFunc main = (args) =>
                {
                    throw new Exception(message);
                };

                TextWriter stderr = new StringWriter();

                Bootstrapper.Run(main, AnyArgs, AnyStdIn, AnyStdOut, stderr, AnyDebugWriter);

                Assert.True(stderr.ToString().Contains(message));
            });

            "The entire uncaught exception chain is written to stderr".Is(() =>
            {
                const string innerMessage = "inner exception";
                const string outerMessage = "outer exception";

                ArgsMainFunc main = (args) =>
                {
                    throw new Exception(outerMessage, new Exception(innerMessage));
                };

                TextWriter stderr = new StringWriter();

                Bootstrapper.Run(main, AnyArgs, AnyStdIn, AnyStdOut, stderr, AnyDebugWriter);

                Assert.True(stderr.ToString().Contains(outerMessage));
                Assert.True(stderr.ToString().Contains(innerMessage));
            });
        }

        [TestCases]
        public static void command_line_parsing()
        {
            "a command line parser can be passed instead of an argument list".Is(() =>
            {
                var anyArguments = new string[0];
                var anySwitches = new Switch[0]; 
                var parser = new CommandLineParser(anyArguments, anySwitches);
                var invoked = false;

                ParserMainFunc main = (_) =>
                {
                    invoked = true;
                    return AnyExitCode;
                };

                Bootstrapper.Run(main, parser, AnyStdIn, AnyStdOut, AnyStdErr, AnyDebugWriter);

                Assert.True(invoked);
            });

            "successfully parsing the command line passes on the parsed arguments".Is(() =>
            {
                var arguments = new[] { "--switch" };
                var switches = new[] { SomeSwitch };
                var parser = new CommandLineParser(arguments, switches);
                var parsed = false;

                ParserMainFunc main = (parsedArgs) =>
                {
                    parsed = parsedArgs.IsSet(SomeSwitchId);
                    return AnyExitCode;
                };

                Bootstrapper.Run(main, parser, AnyStdIn, AnyStdOut, AnyStdErr, AnyDebugWriter);

                Assert.True(parsed);
            });

            "failure to parse the command line returns a non-zero exit code".Is(() =>
            {
                var invalidArgs = new[] { "--invalid-switch" };
                var switches = new[] { SomeSwitch };
                var parser = new CommandLineParser(invalidArgs, switches);
                var invoked = false;

                ParserMainFunc main = (_) =>
                {
                    invoked = true;
                    return ExitCode.Success;
                };

                var exitCode = Bootstrapper.Run(main, parser, AnyStdIn, AnyStdOut, AnyStdErr, AnyDebugWriter);

                Assert.False(invoked);
                Assert.True(exitCode != ExitCode.Success);
            });

            "failure to parse the command line writes an error to stderr".Is(() =>
            {
                var invalidArgs = new[] { "--invalid-switch" };
                var switches = new[] { SomeSwitch };
                var parser = new CommandLineParser(invalidArgs, switches);

                ParserMainFunc main = (_) =>
                {
                    return AnyExitCode;
                };

                TextWriter stderr = new StringWriter();

                Bootstrapper.Run(main, parser, AnyStdIn, AnyStdOut, stderr, AnyDebugWriter);

                Assert.True(stderr.ToString().Contains("invalid-switch"));
            });

            "throwing a command line exception returns a non-zero exit code".Is(() =>
            {
                var arguments = new[] { "--switch" };
                var switches = new[] { SomeSwitch };
                var parser = new CommandLineParser(arguments, switches);
                var invoked = false;

                ParserMainFunc main = (_) =>
                {
                    invoked = true;
                    throw new CmdLineException("error message");
                };

                var exitCode = Bootstrapper.Run(main, parser, AnyStdIn, AnyStdOut, AnyStdErr, AnyDebugWriter);

                Assert.True(invoked);
                Assert.True(exitCode == ExitCode.Failure);
            });

            "throwing a command line exception writes the error to stderr".Is(() =>
            {
                var arguments = new[] { "--switch" };
                var switches = new[] { SomeSwitch };
                var parser = new CommandLineParser(arguments, switches);

                ParserMainFunc main = (_) =>
                {
                    throw new CmdLineException("an error message");
                };

                TextWriter stderr = new StringWriter();

                Bootstrapper.Run(main, parser, AnyStdIn, AnyStdOut, stderr, AnyDebugWriter);

                Assert.True(stderr.ToString().Contains("an error message"));
            });

            "throwing a command line exception writes the usage message to stdout".Is(() =>
            {
                var arguments = new[] { "--switch" };
                var switches = new[] { SomeSwitch };
                var parser = new CommandLineParser(arguments, switches);

                ParserMainFunc main = (_) =>
                {
                    throw new CmdLineException("an error message");
                };

                TextWriter stdout = new StringWriter();

                Bootstrapper.Run(main, parser, AnyStdIn, stdout, AnyStdErr, AnyDebugWriter);

                Assert.True(stdout.ToString().Contains("USAGE"));
            });

            "explicitly requesting help returns a success exit code".Is(() =>
            {
                var arguments = new[] { "--help" };
                var switches = new[] { HelpSwitch };
                var parser = new CommandLineParser(arguments, switches);
                var invoked = false;

                ParserMainFunc main = (_) =>
                {
                    invoked = true;
                    return ExitCode.Failure;
                };

                var exitCode = Bootstrapper.Run(main, parser, AnyStdIn, AnyStdOut, AnyStdErr, AnyDebugWriter);

                Assert.False(invoked);
                Assert.True(exitCode == ExitCode.Success);
            });

            "explicitly requesting help writes the usage message to stdout".Is(() =>
            {
                var arguments = new[] { "--help" };
                var switches = new[] { HelpSwitch };
                var parser = new CommandLineParser(arguments, switches);

                ParserMainFunc main = (_) =>
                {
                    return AnyExitCode;
                };

                TextWriter stdout = new StringWriter();

                Bootstrapper.Run(main, parser, AnyStdIn, stdout, AnyStdErr, AnyDebugWriter);

                Assert.True(stdout.ToString().Contains("USAGE"));
                Assert.True(stdout.ToString().Contains("Tests"));
                Assert.True(stdout.ToString().Contains("--help"));
            });

            "explicitly requesting the version number returns a success exit code".Is(() =>
            {
                var arguments = new[] { "--version" };
                var switches = new[] { VersionSwitch };
                var parser = new CommandLineParser(arguments, switches);
                var invoked = false;

                ParserMainFunc main = (_) =>
                {
                    invoked = true;
                    return ExitCode.Failure;
                };

                var exitCode = Bootstrapper.Run(main, parser, AnyStdIn, AnyStdOut, AnyStdErr, AnyDebugWriter);

                Assert.False(invoked);
                Assert.True(exitCode == ExitCode.Success);
            });

            "explicitly requesting the version number writes the version number to stdout".Is(() =>
            {
                var arguments = new[] { "--version" };
                var switches = new[] { VersionSwitch };
                var parser = new CommandLineParser(arguments, switches);

                ParserMainFunc main = (_) =>
                {
                    return AnyExitCode;
                };

                TextWriter stdout = new StringWriter();

                Bootstrapper.Run(main, parser, AnyStdIn, stdout, AnyStdErr, AnyDebugWriter);

                Assert.True(stdout.ToString().Contains("0.0.0.1"));
            });
        }

        private const int AnyExitCode = ExitCode.Success;
        private static readonly string[] AnyArgs = new string[0];
        private static readonly TextReader AnyStdIn = new StringReader("");
        private static readonly TextWriter AnyStdOut = new StringWriter();
        private static readonly TextWriter AnyStdErr = new StringWriter();
        private static readonly TraceListener AnyDebugWriter = new NullTraceListener();

        private const int SomeSwitchId = 1;
        private static readonly Switch SomeSwitch = new Switch(SomeSwitchId, "s", "switch", "test switch");
        private static readonly Switch HelpSwitch = new Switch(CommandLineParser.HelpSwitch, "h", "help", "Show usage");
        private static readonly Switch VersionSwitch = new Switch(CommandLineParser.VersionSwitch, "v", "version", "Show version");

        private class NullTraceListener : TraceListener
        {
            public override void Write(string message)
            { }

            public override void WriteLine(string message)
            { }
        }
    }
}
