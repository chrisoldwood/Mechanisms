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
    using ParserMainFunc = Func<Arguments, int>;

    public static class Runner
    {
        public static int TestsMain(IEnumerable<string> arguments, Assembly testsAssembly)
        {
            const int helpSwitch = CommandLineParser.HelpSwitch;
            const int versionSwitch = CommandLineParser.VersionSwitch;
            const int verboseSwitch = CommandLineParser.FirstCustomSwitch;

            var switches = new[]
            {
                new Switch(helpSwitch, "h", "help", "Display the program usage"),
                new Switch(versionSwitch, Switch.NoShortName, "version", "Display the program version"),
                new Switch(verboseSwitch, "v", "verbose", "Enable verbose test output"),
            };

            var parser = new CommandLineParser(arguments, switches);

            ParserMainFunc main = (args) => 
            {
                var verbose = args.IsSet(verboseSwitch);

                return RunTests(testsAssembly, args.Unnamed, verbose);
            };

            return Bootstrapper.Run(main, parser);
        }

        private static int RunTests(Assembly testsAssembly, IList<string> filters,  bool verbose)
        {
            var warnings = new List<string>();

            RegisterTestCases(testsAssembly, warnings);

            var successes = 0;
            var failures = 0;
            var unknown = 0;
            var nonSuccesses = new List<Suite.TestCase>();

            foreach (var test in Suite.TestCases)
            {
                if (filters.Any() && !Included(filters, test))
                    continue;

                if (verbose)
                    Console.WriteLine("[{0}] {1}".Fmt(test.Type, test.Name));

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

            if (warnings.Any())
            {
                foreach (var warning in warnings)
                {
                    DebugWriter.WriteLine(warning);
                    Console.WriteLine(warning);
                }

                Console.WriteLine();
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
                            Suite.TestCaseAdder. CurrentType = FormatTypeName(method);
                            method.Invoke(null, new object[0]);
                            Suite.TestCaseAdder.CurrentType = null;
                        }
                        else
                        {
                            var name = FormatTypeName(method);
                            var warning = String.Format("WARNING: [TestCases] method '{0}' is not declared static.", name);
                            warnings.Add(warning);
                        }
                    }
                }
            }

            var duplicates = Suite.TestCases.GroupBy(tc => tc.Type + "|" + tc.Name)
                                            .Where(g => g.Count() > 1)
                                            .Select(g => g.Key);

            foreach (var duplicate in duplicates)
            {
                var warning = String.Format("WARNING: duplicate test case '{0}' detected.", duplicate);
                warnings.Add(warning);
            }
        }

        private static bool Included(IEnumerable<string> filters, Suite.TestCase testCase)
        {
            return filters.Any(f => testCase.Type.Contains(f) || testCase.Name.Contains(f));
        }

        private static string FormatTypeName(MethodInfo method)
        {
            // ReSharper disable PossibleNullReferenceException
            return method.DeclaringType.Name + "|" + method.Name;
            // ReSharper restore PossibleNullReferenceException
        }

        private static string FormatStackFrame(StackFrame stackFrame)
        {
            var filename = Path.GetFileName(stackFrame.GetFileName());

            return "[{0}, {1}]".Fmt(filename, stackFrame.GetFileLineNumber());
        }

        private static readonly DefaultTraceListener DebugWriter = new DefaultTraceListener();
    }
}
