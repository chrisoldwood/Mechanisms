namespace Mechanisms.Extensions
{
    public static class StringExtensions
    {
        public static bool IsEmpty(this string value)
        {
            return value.Length == 0;
        }
    }
}
