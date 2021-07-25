using System;
using System.IO;

namespace ENCODE.Base
{
    static class Logger
    {
        static private TextWriter writer;
        static private string separatorChar = "--------------------------------------------------------------------------------------------";

        static Logger()
        {
            writer = File.CreateText(Constant.LOG_FILE_PATH);
        }

        static public void Write(string line)
        {
            Console.Write(line);
            writer.Write(line);
            writer.Flush();
        }

        static public void ReWrite(string line)
        {
            Console.Write($"\r{line}    ");
        }


        static public void WriteLine(string line)
        {
            Console.WriteLine(line);
            writer.WriteLine(line);
            writer.Flush();
        }

        static public void WriteLineToConsoleOnly(string line)
        {
            Console.WriteLine(line);
        }

        static public void WriteLineToFileOnly(string line)
        {
            writer.WriteLine(line);
            writer.Flush();
        }
        static public void WriteLineDebug(string line)
        {
            writer.WriteLine(line);
            writer.Flush();

            if (Constant.DEBUG_OUTPUT)
                Console.WriteLine(line);
        }

        static public void Separator()
        {
            Console.WriteLine(separatorChar);
            writer.WriteLine(separatorChar);
            writer.Flush();
        }

    }
}
