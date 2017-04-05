namespace Mechanisms.Contracts
{
    public static class Expect
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

        public static void NotNull([NotNull] object value, string variable)
        {
            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            True(value != null, variable + " should not be null");
            // ReSharper restore ConditionIsAlwaysTrueOrFalse
        }

        public static void NotEmpty([NotNull] string value, string variable)
        {
            False(string.IsNullOrEmpty(value), variable + " should not be null or empty");
        }
    }
}
