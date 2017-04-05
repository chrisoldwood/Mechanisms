using System;
using Mechanisms.Extensions;
using Mechanisms.Tests;

// Not  interested in localisation issues here.
// ReSharper disable StringIndexOfIsCultureSpecific.1

namespace Tests.Extensions
{
    public static class ExceptionExtensionsTests
    {
        [TestCases]
        public static void message_formatting()
        {
            "Exception message includes the exception's full type name".Is(() =>
            {
                var exception = new ApplicationException();

                var message = exception.FormatMessage();

                Assert.True(message.Contains("[System.ApplicationException]"));
            });

            "Exception message includes the exception's message if present".Is(() =>
            {
                const string exceptionMessage = "bad things happened";

                var exception = new ApplicationException(exceptionMessage);

                var message = exception.FormatMessage();

                Assert.True(message.Contains(exceptionMessage));
            });

            "Exception message contains just the type name if no message is present".Is(() =>
            {
                const string noMessage = "";

                var exception = new Exception(noMessage);

                var message = exception.FormatMessage();

                Assert.True(message.Equals("[System.Exception]"));
            });

            "Exception message includes any inner exceptions, when present".Is(() =>
            {
                const string innerMessage = "inner exception";
                const string outerMessage = "outer exception";

                var innerException = new Exception(innerMessage);
                var outerException = new ApplicationException(outerMessage, innerException);

                var message = outerException.FormatMessage();

                Assert.True(message.Contains("[System.Exception]"));
                Assert.True(message.Contains(innerMessage));
                Assert.True(message.Contains("[System.ApplicationException]"));
                Assert.True(message.Contains(outerMessage));
            });

            "Exception message reads from outermost to innermost".Is(() =>
            {
                const string innermost = "inner";
                const string outermost = "outer";
                const string inbetween = "in-between";

                var innerException = new Exception(innermost);
                var inbetweenException = new Exception(inbetween, innerException);
                var outerException = new Exception(outermost, inbetweenException);

                var message = outerException.FormatMessage();

                var innermostPos = message.IndexOf(innermost);
                var inbetweenPos = message.IndexOf(inbetween);
                var outermostPos = message.IndexOf(outermost);

                Assert.True((innermostPos != -1) && (inbetweenPos != -1) && (outermostPos != -1));
                Assert.True(innermostPos > inbetweenPos);
                Assert.True(inbetweenPos > outermostPos);
            });
        }
    }
}
