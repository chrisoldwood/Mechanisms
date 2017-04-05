using System;
using System.Text;

namespace Mechanisms.Extensions
{
    public static class ExceptionExtensions
    {
        public static string FormatMessage(this Exception exception)
        {
            var messageBuilder = new StringBuilder();

            messageBuilder.AppendFormat("[{0}]", FormatTypeName(exception));
        
            if (!String.IsNullOrEmpty(exception.Message))
                messageBuilder.AppendFormat(" {0}", exception.Message);

            if (exception.InnerException != null)
                messageBuilder.AppendFormat(" | {0}", exception.InnerException.FormatMessage());

            return messageBuilder.ToString();
        }

        private static string FormatTypeName(object @object)
        {
            var type = @object.GetType();

            return type.Namespace + "." + type.Name;
        }
    }
}
