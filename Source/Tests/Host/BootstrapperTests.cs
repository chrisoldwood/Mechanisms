using System;
using System.Diagnostics;
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

                var exitCode = Bootstrapper.Run(main, AnyArgs, AnyStdIn, AnyStdOut, AnyStdErr, AnyDebugWriter);

                Assert.True(exitCode == mainExitCode);
            });

            "The boostrapper returns a non-zero value when an exception is not caught".Is(() =>
            {
                MainFunc main = (args) =>
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

                MainFunc main = (args) =>
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

                MainFunc main = (args) =>
                {
                    throw new Exception(outerMessage, new Exception(innerMessage));
                };

                TextWriter stderr = new StringWriter();

                Bootstrapper.Run(main, AnyArgs, AnyStdIn, AnyStdOut, stderr, AnyDebugWriter);

                Assert.True(stderr.ToString().Contains(outerMessage));
                Assert.True(stderr.ToString().Contains(innerMessage));
            });
        }

        private static readonly string[] AnyArgs = new string[0];
        private static readonly TextReader AnyStdIn = new StringReader("");
        private static readonly TextWriter AnyStdOut = new StringWriter();
        private static readonly TextWriter AnyStdErr = new StringWriter();
        private static readonly TraceListener AnyDebugWriter = new NullTraceListener();

        private class NullTraceListener : TraceListener
        {
            public override void Write(string message)
            { }

            public override void WriteLine(string message)
            { }
        }
    }
}
