using System;
using System.IO;

namespace FluentLogger.Console
{
    using Console = System.Console;

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("You must provide a file");
                return;
            }
            using (var stream = new FileStream(args[0], FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var lineReader = new StreamReader(stream))
                {
                    ConsoleColor color = ConsoleColor.White;
                    string line = lineReader.ReadLine();
                    while (true)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            if (line.Contains("[WARN]"))
                                color = ConsoleColor.Yellow;
                            else if (line.Contains("[TRACE]"))
                                color = ConsoleColor.White;
                            else if (line.Contains("[Info]"))
                                color = ConsoleColor.Green;
                            else if (line.Contains("[Critical]"))
                                color = ConsoleColor.Magenta;
                            else if (line.Contains("[ERROR]"))
                                color = ConsoleColor.Red;
                            else if (line.Contains("[FATAL]"))
                                color = ConsoleColor.DarkRed;

                            Console.ForegroundColor = color;
                            Console.WriteLine(line);

                        }
                        line = lineReader.ReadLine();
                    }
                }
            }
        }
    }
}
