using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Mechanisms.Extensions;

namespace Mechanisms.Host
{
    using ArgsMainFunc = Func<string[], int>;
    using ParserMainFunc = Func<Arguments, int>;

    public class Bootstrapper
    {
        public static int Run(ArgsMainFunc main, string[] args)
        {
            var debugWriter = new DefaultTraceListener();

            return Run(main, args, Console.In, Console.Out, Console.Error, debugWriter);
        }

        public static int Run(ArgsMainFunc main, string[] args,
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

        public static int Run(ParserMainFunc main, CommandLineParser parser)
        {
            var debugWriter = new DefaultTraceListener();

            return Run(main, parser, Console.In, Console.Out, Console.Error, debugWriter);
        }

        public static int Run(ParserMainFunc main, CommandLineParser parser,
                              TextReader stdin, TextWriter stdout, TextWriter stderr,
                              TraceListener debugWriter)
        {
            try
            {
                var arguments = parser.Parse();

                if (arguments.IsSet(CommandLineParser.HelpSwitch))
                {
                    WriteUsage(parser, stdout);
                    return ExitCode.Success;
                }
                else if (arguments.IsSet(CommandLineParser.VersionSwitch))
                {
                    stdout.WriteLine(Assembly.GetEntryAssembly().GetName().Version.ToString());
                    return ExitCode.Success;
                }

                return main(arguments);
            }
            catch (CmdLineException e)
            {
                var message = "ERROR: {0}".Fmt(e.Message);
                stderr.WriteLine(message);
                WriteUsage(parser, stdout);
                return ExitCode.Failure;
            }
            catch (Exception e)
            {
                var message = e.FormatMessage();
                debugWriter.WriteLine(message);
                stderr.WriteLine(message);
                return ExitCode.Failure;
            }
        }

        private static void WriteUsage(CommandLineParser parser, TextWriter stdout)
        {
            var name = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location);

            stdout.WriteLine();
            stdout.WriteLine("USAGE: {0} [options...]".Fmt(name));
            stdout.WriteLine();
            foreach (var line in parser.FormatSwitches())
                stdout.WriteLine(line);
        }
    }
}
