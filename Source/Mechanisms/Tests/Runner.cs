using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Mechanisms.Contracts;
using Mechanisms.Host;

namespace Mechanisms.Tests
{
    public static class Runner
    {
        public static int Run(Assembly testsAssembly)
        {
            RegisterTestCases(testsAssembly);

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
                catch (Exception)
                {
                    test.RecordAssert(false);
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
                    var message = outcome + ": " + test.Name;

                    DebugWriter.WriteLine(message);
                    Console.WriteLine(message);
                }

                Console.Write("\n");
            }

            var summary = String.Format("Test Results: {0} Passed {1} Failed {2} Unknown",
                                        successes, failures, unknown);
            DebugWriter.WriteLine(summary);
            Console.WriteLine(summary);

            return (failures == 0) ? ExitCode.Success : ExitCode.Failure;
        }

        private static void RegisterTestCases(Assembly testsAssembly)
        {
            Type[] types = testsAssembly.GetTypes();

            foreach (var type in types)
            {
                MemberInfo[] members = type.GetMembers(BindingFlags.Static|BindingFlags.Public);

                foreach (var member in members)
                {
                    Enforce.True(member is MethodInfo, "member is MethodInfo");

                    Attribute[] attributes = Attribute.GetCustomAttributes(member, typeof(TestCasesAttribute));

                    Enforce.True(attributes.Length == 1, "attributes.Length == 1");

                    ((MethodInfo)member).Invoke(null, new object[0]);
                }
            }
        }

        private static readonly DefaultTraceListener DebugWriter = new DefaultTraceListener();
    }
}
