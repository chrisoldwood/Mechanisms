using System;
using System.Diagnostics;
using System.IO;

namespace Mechanisms.Host
{
    public class IOStreams
    {
        public TextReader StdIn { get; private set; }
        public TextWriter StdOut { get; private set; }
        public TextWriter StdErr { get; private set; }
        public TraceListener DebugWriter { get; private set; }

        public IOStreams()
            : this(Console.In, Console.Out, Console.Error, new DefaultTraceListener())
        {
        }

        public IOStreams(TextReader stdin, TextWriter stdout, TextWriter stderr)
            : this(stdin, stdout, stderr, NopTraceListener)
        {
        }

        public IOStreams(TextReader stdin, TextWriter stdout, TextWriter stderr,
                         TraceListener debugWriter)
        {
            StdIn = stdin;
            StdOut = stdout;
            StdErr  = stderr;
            DebugWriter = debugWriter;
        }

        private class NullTraceListener : TraceListener
        {
            public override void Write(string message)
            { }

            public override void WriteLine(string message)
            { }
        }

        private static readonly NullTraceListener NopTraceListener = new NullTraceListener();
    }
}
