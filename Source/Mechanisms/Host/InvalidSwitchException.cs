using System;

namespace Mechanisms.Host
{
    public class InvalidSwitchException : Exception
    {
        public InvalidSwitchException(string message)
            : base(message)
        {
        }
    }
}
