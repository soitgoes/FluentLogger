using System;
using System.IO;
using System.Threading.Tasks;

namespace FluentLogger.Console
{
    using Console = System.Console;

    public class LogWatcher
    {
        private readonly string[] args;

        public LogWatcher(string[] args)
        {
            this.args = args;
        }
        public async Task Run()
        {
            foreach (var item in args)
            {
                var isDirectory = Directory.Exists(item);
                if (isDirectory)
                {
                    var files = Directory.GetFiles(item);
                    foreach (var file in files)
                    {
                        await WatchFile(file);
                    }
                }
                else
                {
                    if (File.Exists(item))
                    {
                        await WatchFile(item);
                    }
                }
            }
        }

        private static async Task WatchFile(string filename)
        {
            await Task.Run(() =>
            {
                using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
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
            });

        }
    }
}
