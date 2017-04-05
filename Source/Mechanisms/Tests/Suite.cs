using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Mechanisms.Tests
{
    public static class Suite
    {
        public static void Add(string name, Action function)
        {
            Tests.Add(new TestCase(name, function));
        }
        
        internal static IEnumerable<TestCase> TestCases
        {
            get { return Tests; }
        }

        internal class TestCase
        {
            public string Name { get; private set;  }
            public Action Function { get; private set;  }

            public int Successes { get; private set; }
            public int Failures { get; private set; }

            public StackFrame FirstFailure { get; private set; }

            public TestCase(string name, Action function)
            {
                Name = name;
                Function = function;
            }

            public void RecordPass()
            {
                ++Successes;
            }

            public void RecordFailure(StackFrame caller)
            {
                ++Failures;

                if ((FirstFailure == null) && (caller != null))
                    FirstFailure = caller;
            }
        }

        private static readonly List<TestCase> Tests = new List<TestCase>();
    }

    public static class StringExtensions
    {
        public static void Is(this string name, Action function)
        {
            Suite.Add(name, function);
        }
    }
}
