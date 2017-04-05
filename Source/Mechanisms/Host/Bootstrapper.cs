using System;
using System.IO;

namespace Mechanisms.Host
{
    using MainFunc = Func<string[], int>;

    public class Bootstrapper
    {
        public static int Run(MainFunc main, string[] args)
        {
            return Run(main, args, Console.In, Console.Out, Console.Error);
        }

        public static int Run(MainFunc main, string[] args, TextReader stdin, TextWriter stdout, TextWriter stderr)
        {
            try
            {
                return main(args);
            }
            catch (Exception e)
            {
                stderr.WriteLine(e.Message);
                return ExitCode.Failure;
            }
        }
    }
}
