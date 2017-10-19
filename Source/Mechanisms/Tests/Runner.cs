using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Mechanisms.Extensions;
using Mechanisms.Host;
using Switch = Mechanisms.Host.Switch;

namespace Mechanisms.Tests
{
    public static class Runner
    {
        public static int Run(Assembly testsAssembly)
        {
            return Run(testsAssembly, Enumerable.Empty<string>());
        }

        public static int Run(Assembly testsAssembly, IEnumerable<string> args)
        {
            const int helpSwitch = 1;
            const int verboseSwitch = 2;

            var switches = new[]
            {
                new Switch(helpSwitch, "h", "help"),
                new Switch(verboseSwitch, "v", "verbose"),
            };

            var arguments = CommandLineParser.Parse(args, switches);

            if (arguments.IsSet(helpSwitch))
            {
                var runner = Path.GetFileName(testsAssembly.Location);
                Console.WriteLine("USAGE: {0} [options...]", runner);
                return ExitCode.Success;
            }

            var verbose = arguments.IsSet(verboseSwitch);
            var warnings = new List<string>();

            RegisterTestCases(testsAssembly, warnings);

            var successes = 0;
            var failures = 0;
            var unknown = 0;
            var nonSuccesses = new List<Suite.TestCase>();

            foreach (var test in Suite.TestCases)
            {
                if (verbose)
                    Console.WriteLine(test.Name);

                Assert.OnTestCaseStart(test);

                try
                {
                    test.Function();
                }
                #pragma warning disable 168
                catch (Exception e)
                #pragma warning restore 168
                {
                    test.RecordFailure(null);
                }

                if (test.Failures != 0)
                {
                    if (!verbose) 
                        Console.Write("F");
                    ++failures;
                    nonSuccesses.Add(test);
                }
                else if (test.Successes == 0)
                {
                    if (!verbose) 
                        Console.Write("U");
                    ++unknown;
                    nonSuccesses.Add(test);
                }
                else
                {
                    if (!verbose) 
                        Console.Write(".");
                    ++successes;
                }                
            }
            Console.Write(verbose ? "\n" : "\n\n");

            if (nonSuccesses.Any())
            {
                foreach (var test in nonSuccesses)
                {
                    var outcome = (test.Failures != 0) ? "FAILED " : "UNKNOWN";
                    var caller = (test.FirstFailure != null) ? FormatStackFrame(test.FirstFailure) : "";
                    var message = outcome + ": " + test.Name + " " + caller;
                    

                    DebugWriter.WriteLine(message);
                    Console.WriteLine(message);
                }

                Console.Write("\n");
            }

            foreach (var warning in warnings)
            {
                DebugWriter.WriteLine(warning);
                Console.WriteLine(warning);
            }

            var summary = String.Format("Test Results: {0} Passed {1} Failed {2} Unknown",
                                        successes, failures, unknown);
            DebugWriter.WriteLine(summary);
            Console.WriteLine(summary);

            return (failures == 0) ? ExitCode.Success : ExitCode.Failure;
        }

        private static void RegisterTestCases(Assembly testsAssembly, IList<string> warnings)
        {
            var types = testsAssembly.GetTypes();

            foreach (var type in types)
            {
                var methods = type.GetMethods();

                foreach (var method in methods)
                {
                    var attributes = Attribute.GetCustomAttributes(method, typeof(TestCasesAttribute));

                    if (attributes.Length == 1)
                    {
                        if (method.IsStatic)
                        {
                            method.Invoke(null, new object[0]);
                        }
                        else
                        {
                            // ReSharper disable PossibleNullReferenceException
                            var name = method.DeclaringType.Name + "." + method.Name;
                            var warning = String.Format("WARNING: [TestCases] method '{0}' is not declared static.", name);
                            warnings.Add(warning);
                            // ReSharper restore PossibleNullReferenceException
                        }
                    }
                }
            }
        }

        private static string FormatStackFrame(StackFrame stackFrame)
        {
            var filename = Path.GetFileName(stackFrame.GetFileName());

            return "[{0}, {1}]".Fmt(filename, stackFrame.GetFileLineNumber());
        }

        private static readonly DefaultTraceListener DebugWriter = new DefaultTraceListener();
    }
}
