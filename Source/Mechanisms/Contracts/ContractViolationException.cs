using System;

namespace Mechanisms.Contracts
{
    public class ContractViolationException : Exception
    {
        public ContractViolationException(string message)
            : base(message)
        {
        }
    }
}
