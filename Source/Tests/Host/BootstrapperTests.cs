using System;
using System.IO;
using Mechanisms.Host;
using Mechanisms.Tests;

// We want our lambdas to look like real methods.
// ReSharper disable ConvertToLambdaExpression
// ReSharper disable RedundantLambdaSignatureParentheses

namespace Tests.Host
{
    using MainFunc = Func<string[], int>;

    public static class BootstrapperTests
    {
        [TestCases]
        public static void exit_codes()
        {
            "By default the boostrapper passes through the exit code from main()".Is(() =>
            {
                const int mainExitCode = 99;

                MainFunc main = (args) =>
                {
                    return mainExitCode;
                };

                var exitCode = Bootstrapper.Run(main, AnyArgs, AnyStdIn, AnyStdOut, AnyStdErr);

                Assert.True(exitCode == mainExitCode);
            });

            "The boostrapper returns a non-zero value when an exception is not caught".Is(() =>
            {
                MainFunc main = (args) =>
                {
                    throw new Exception("any exception");
                };

                var exitCode = Bootstrapper.Run(main, AnyArgs, AnyStdIn, AnyStdOut, AnyStdErr);

                Assert.True(exitCode != 0);
            });
        }

        [TestCases]
        public static void error_reporting()
        {
            "The message for an uncaught exception is written to stderr".Is(() =>
            {
                const string message = "an exception message";

                MainFunc main = (args) =>
                {
                    throw new Exception(message);
                };

                TextWriter stderr = new StringWriter();

                Bootstrapper.Run(main, AnyArgs, AnyStdIn, AnyStdOut, stderr);

                Assert.True(stderr.ToString().Contains(message));
            });
        }

        private static readonly string[] AnyArgs = new string[0];
        private static readonly TextReader AnyStdIn = new StringReader("");
        private static readonly TextWriter AnyStdOut = new StringWriter();
        private static readonly TextWriter AnyStdErr = new StringWriter();
    }
}
