using System;
using System.IO;
using System.Reflection;
using Mechanisms.Extensions;

namespace Mechanisms.Host
{
    using ArgsMainFunc = Func<string[], int>;
    using ArgsStreamsMainFunc = Func<string[], IOStreams, int>;
    using ParserMainFunc = Func<Arguments, int>;
    using ParserStreamsMainFunc = Func<Arguments, IOStreams, int>;

    public static class Bootstrapper
    {
        public static int Run(ArgsMainFunc main, string[] arguments)
        {
            ArgsStreamsMainFunc mainAdapter = (args, _) => main(args);

            return Run(mainAdapter, arguments, new IOStreams());
        }

        public static int Run(ArgsMainFunc main, string[] arguments, IOStreams streams)
        {
            ArgsStreamsMainFunc mainAdapter = (args, _) => main(args);

            return Run(mainAdapter, arguments, streams);
        }

        public static int Run(ArgsStreamsMainFunc main, string[] args, IOStreams streams)
        {
            try
            {
                return main(args, streams);
            }
            catch (Exception e)
            {
                var message = e.FormatMessage();
                streams.DebugWriter.WriteLine(message);
                streams.StdErr.WriteLine(message);
                return ExitCode.Failure;
            }
        }

        public static int Run(ParserMainFunc main, CommandLineParser parser)
        {
            ParserStreamsMainFunc mainAdapter = (args, _) => main(args);

            return Run(mainAdapter, parser, new IOStreams());
        }

        public static int Run(ParserMainFunc main, CommandLineParser parser, IOStreams streams)
        {
            ParserStreamsMainFunc mainAdapter = (args, _) => main(args);

            return Run(mainAdapter, parser, streams);
        }

        public static int Run(ParserStreamsMainFunc main, CommandLineParser parser, IOStreams streams)
        {
            try
            {
                var arguments = parser.Parse();

                if (arguments.IsSet(CommandLineParser.HelpSwitch))
                {
                    WriteUsage(parser, streams.StdOut);
                    return ExitCode.Success;
                }
                else if (arguments.IsSet(CommandLineParser.VersionSwitch))
                {
                    streams.StdOut.WriteLine(Assembly.GetEntryAssembly().GetName().Version.ToString());
                    return ExitCode.Success;
                }

                return main(arguments, streams);
            }
            catch (CmdLineException e)
            {
                var message = "ERROR: {0}".Fmt(e.Message);
                streams.StdErr.WriteLine(message);
                WriteUsage(parser, streams.StdOut);
                return ExitCode.Failure;
            }
            catch (Exception e)
            {
                var message = e.FormatMessage();
                streams.DebugWriter.WriteLine(message);
                streams.StdErr.WriteLine(message);
                return ExitCode.Failure;
            }
        }

        private static void WriteUsage(CommandLineParser parser, TextWriter stdout)
        {
            var name = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location);

            stdout.WriteLine();
            stdout.WriteLine("USAGE: {0} [options...]".Fmt(name));
            stdout.WriteLine();
            foreach (var line in parser.Grammar.FormatSwitches())
                stdout.WriteLine(line);
        }
    }
}
