using System.Reflection;
using Mechanisms.Tests;

namespace Tests
{
    public static class Program
    {
        static int Main(string[] args)
        {
            return Runner.TestsMain(args, Assembly.GetExecutingAssembly());
        }
    }
}
