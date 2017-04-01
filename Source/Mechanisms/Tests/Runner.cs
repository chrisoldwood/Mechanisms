using System;
using System.Collections.Generic;
using System.Linq;

namespace Mechanisms.Tests
{
    public static class Runner
    {
        public static int Run()
        {
            var successes = 0;
            var failures = 0;
            var unknown = 0;
            var nonSuccesses = new List<Suite.TestCase>();

            foreach (var test in Suite.TestCases)
            {
                Assert.OnTestCaseStart(test);

                test.Function();

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

                    Console.WriteLine(outcome + ": " + test.Name);
                }

                Console.Write("\n");
            }

            var summary = String.Format("Test Results: {0} Passed {1} Failed {2} Unknown",
                                        successes, failures, unknown);

            Console.WriteLine(summary);
            return (failures == 0) ? 0 : 1;
        }
    }
}
