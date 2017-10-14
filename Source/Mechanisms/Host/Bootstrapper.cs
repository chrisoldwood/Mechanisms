using System;
using System.Diagnostics;
using System.IO;
using Mechanisms.Extensions;

namespace Mechanisms.Host
{
    using MainFunc = Func<string[], int>;

    public class Bootstrapper
    {
        public static int Run(MainFunc main, string[] args)
        {
            var debugWriter = new DefaultTraceListener();

            return Run(main, args, Console.In, Console.Out, Console.Error, debugWriter);
        }

        public static int Run(MainFunc main, string[] args,
                              TextReader stdin, TextWriter stdout, TextWriter stderr,
                              TraceListener debugWriter)
        {
            try
            {
                return main(args);
            }
            catch (Exception e)
            {
                var message = e.FormatMessage();
                debugWriter.WriteLine(message);
                stderr.WriteLine(message);
                return ExitCode.Failure;
            }
        }
    }
}
