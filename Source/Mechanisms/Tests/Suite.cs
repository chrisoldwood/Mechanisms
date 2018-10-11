using System;
using System.Collections.Generic;
using System.Diagnostics;
using Mechanisms.Contracts;

namespace Mechanisms.Tests
{
    internal static class Suite
    {
        public static IEnumerable<TestCase> TestCases
        {
            get { return Tests; }
        }

        internal class TestCase
        {
            public string Type { get; private set;  }
            public string Name { get; private set;  }
            public Action Function { get; private set;  }

            public int Successes { get; private set; }
            public int Failures { get; private set; }

            public StackFrame FirstFailure { get; private set; }

            public TestCase(string type, string name, Action function)
            {
                Type = type;
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

        internal static class TestCaseAdder
        {
            public static string CurrentType { private get; set; }

            public static void AddCase(string name, Action function)
            {
                Expect.NotEmpty(CurrentType, "CurrentType");

                Tests.Add(new TestCase(CurrentType, name, function));
            }
        }

        private static readonly List<TestCase> Tests = new List<TestCase>();
    }

    public static class StringExtensions
    {
        public static void Is(this string name, Action function)
        {
            Suite.TestCaseAdder.AddCase(name, function);
        }
    }
}
