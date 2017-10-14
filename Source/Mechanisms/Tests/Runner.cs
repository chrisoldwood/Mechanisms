using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Mechanisms.Extensions;
using Mechanisms.Host;

namespace Mechanisms.Tests
{
    public static class Runner
    {
        public static int Run(Assembly testsAssembly)
        {
            var warnings = new List<string>();

            RegisterTestCases(testsAssembly, warnings);

            var successes = 0;
            var failures = 0;
            var unknown = 0;
            var nonSuccesses = new List<Suite.TestCase>();

            foreach (var test in Suite.TestCases)
            {
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
                    Console.Write("F");
                    ++failures;
                    nonSuccesses.Add(test);
                }
                else if (test.Successes == 0)
                {
                    Console.Write("U");
                    ++unknown;
                    nonSuccesses.Add(test);
                }
                else
                {
                    Console.Write(".");
                    ++successes;
                }                
            }
            Console.Write("\n\n");

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
