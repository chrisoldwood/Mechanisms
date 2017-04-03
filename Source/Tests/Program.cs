using Mechanisms.Tests;
using Tests.Extensions;

namespace Tests
{
    public class Program
    {
        static int Main(string[] args)
        {
            EnumerableExtensionsTests.Define();
            StringExtensionsTests.Define();

            return Runner.Run();
        }
    }
}
