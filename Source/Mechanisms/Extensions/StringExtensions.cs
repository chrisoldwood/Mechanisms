namespace Mechanisms.Extensions
{
    public static class StringExtensions
    {
        public static bool IsEmpty(this string value)
        {
            return value.Length == 0;
        }

        public static string Fmt(this string format, params object[] args)
        {
            return string.Format(format, args);
        }
    }
}
