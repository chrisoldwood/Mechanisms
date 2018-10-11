using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Mechanisms.Tests
{
    public static class Assert
    {
        public static void Pass()
        {
            _currentTest.RecordPass();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void True(bool result)
        {
            if (result)
            {
                _currentTest.RecordPass();
            }
            else
            {
                var caller = new StackFrame(1, true);
                _currentTest.RecordFailure(caller);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void False(bool result)
        {
            if (!result)
            {
                _currentTest.RecordPass();
            }
            else
            {
                var caller = new StackFrame(1, true);
                _currentTest.RecordFailure(caller);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Throws(Action statement)
        {
            try
            {
                statement();

                var caller = new StackFrame(1, true);
                _currentTest.RecordFailure(caller);
            }
            catch (Exception)
            {
                _currentTest.RecordPass();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static T Throws<T>(Action statement)
            where T : Exception
        {
            try
            {
                statement();

                var caller = new StackFrame(1, true);
                _currentTest.RecordFailure(caller);
            }
            catch (T e)
            {
                return e;
            }
            catch (Exception)
            {
                _currentTest.RecordPass();
            }

            return null;
        }

        internal static void OnTestCaseStart(Suite.TestCase test)
        {
            _currentTest = test;
        }

        private static Suite.TestCase _currentTest;
    }
}
