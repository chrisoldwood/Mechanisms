using System;

namespace Mechanisms.Tests
{
    public static class Assert
    {
        public static void Pass()
        {
            _currentTest.RecordPass();
        }

        public static void True(bool result)
        {
            _currentTest.RecordAssert(result);
        }

        public static void False(bool result)
        {
            _currentTest.RecordAssert(!result);
        }

        public static void Throws(Action statement)
        {
            try
            {
                statement();
                _currentTest.RecordAssert(false);
            }
            catch (Exception)
            {
                _currentTest.RecordAssert(true);
            }
        }

        internal static void OnTestCaseStart(Suite.TestCase test)
        {
            _currentTest = test;
        }

        private static Suite.TestCase _currentTest;
    }
}
