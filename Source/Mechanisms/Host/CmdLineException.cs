using System;

namespace Mechanisms.Host
{
    public class CmdLineException : Exception
    {
        public CmdLineException(string message)
            : base(message)
        {
        }
    }
}
