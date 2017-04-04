namespace Mechanisms.Contracts
{
    public static class Enforce
    {
        public static void True(bool result, string expression)
        {
            if (!result)
            {
                var message = "Contract violation: " + expression;

                throw new ContractViolationException(message);
            }
        }

        public static void False(bool result, string expression)
        {
            True(!result, expression);
        }

        public static void NotNull(object value, string variable)
        {
            True(value != null, variable + " should not be null");
        }

        public static void NotEmpty(string value, string variable)
        {
            False(string.IsNullOrEmpty(value), variable + " should not be null or empty");
        }
    }
}
