using System;
using System.Collections.Generic;

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

            public TestCase(string name, Action function)
            {
                Name = name;
                Function = function;
            }

            public void RecordAssert(bool succeeded)
            {
                if (succeeded)
                    ++Successes;
                else
                    ++Failures;
            }
        }

        private static readonly List<TestCase> Tests = new List<TestCase>();
    }
}
