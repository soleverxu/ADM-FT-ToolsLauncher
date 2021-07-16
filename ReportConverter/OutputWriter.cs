using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReportConverter
{
    static class OutputWriter
    {
        private static readonly Assembly _progAssembly = Assembly.GetEntryAssembly();

        private static TextWriter Writer { get; set; }

        public static int VerboseLevel { get; private set; }

        static OutputWriter()
        {
            Writer = Console.Out;
            VerboseLevel = 0;
        }

        public static void SetVerboseLevel(int level = 0)
        {
            VerboseLevel = level;
        }

        public static void SetWriter(TextWriter w)
        {
            Writer = w == null ? TextWriter.Null : w;
        }

        public static void WriteTitle()
        {
            Writer.Write(Properties.Resources.Prog_Title);
            Writer.Write(" ");
            WriteVersion();
            Writer.WriteLine();
        }

        public static void WriteVersion()
        {
            Writer.WriteLine(_progAssembly.GetName().Version.ToString());
        }

        public static void WriteCommandUsage(params string[] messages)
        {
            if (messages.Length > 0)
            {
                WriteLines(messages);
                Writer.WriteLine();
            }

            ArgHelper.Instance.WriteUsage(Writer, _progAssembly.GetName().Name);
        }

        public static void WriteLines(params string[] lines)
        {
            if (lines.Length > 0)
            {
                foreach (string line in lines)
                {
                    Writer.WriteLine(line);
                }
            }
        }

        public static void WriteLine()
        {
            Writer.WriteLine();
        }

        public static void WriteLine(string value)
        {
            Writer.WriteLine(value);
        }

        public static void WriteLine(string format, params object[] arg)
        {
            Writer.WriteLine(format, arg);
        }

        public static void WriteVerboseLine(OutputVerboseLevel level)
        {
            // the verbose level set by the program is smaller than the required one
            // it means no need to write this verbose line and it should be skipped
            if (VerboseLevel < (int)level)
            {
                return;
            }

            WriteLine();
        }

        public static void WriteVerboseLine(OutputVerboseLevel level, string value)
        {
            // the verbose level set by the program is smaller than the required one
            // it means no need to write this verbose line and it should be skipped
            if (VerboseLevel < (int)level)
            {
                return;
            }

            WriteLine(value);
        }

        public static void WriteVerboseLine(OutputVerboseLevel level, string format, params object[] arg)
        {
            // the verbose level set by the program is smaller than the required one
            // it means no need to write this verbose line and it should be skipped
            if (VerboseLevel < (int)level)
            {
                return;
            }

            WriteLine(format, arg);
        }

        public static void WriteVerboseLines(OutputVerboseLevel level, params string[] lines)
        {
            // the verbose level set by the program is smaller than the required one
            // it means no need to write this verbose line and it should be skipped
            if (VerboseLevel < (int)level)
            {
                return;
            }

            WriteLines(lines);
        }
    }

    enum OutputVerboseLevel : int
    {
        None = 0,
        Verbose,
        ExtraVerbose
    }
}
