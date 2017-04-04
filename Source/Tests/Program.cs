using System.Reflection;
using Mechanisms.Tests;

namespace Tests
{
    public class Program
    {
        static int Main(string[] args)
        {
            return Runner.Run(Assembly.GetExecutingAssembly());
        }
    }
}
