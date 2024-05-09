using System;
using System.Diagnostics;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Globalization;

namespace FluentLogger
{


    [Obsolete("Use MaximumFileSizeRoller Instead")]
    public class DailyLogRoller : BaseLogger
    {
        protected static object hold = new object();
        protected static string logDirectory = Path.Combine(Environment.CurrentDirectory, "Logs");
        protected static string filePath = null;
        protected int counter = 0;
        protected static string day = null;
       

        public Func<string> FilenameFx = new Func<string>(() =>
        {
            return $"log-" + DateTime.UtcNow.ToString("yyyy-MM-dd") + $".{pid}.txt";
        });

        public DailyLogRoller(string directoryForLog, LogLevel minLevel) : base(minLevel)
        {
            logDirectory = directoryForLog;
            if (!Directory.Exists(directoryForLog))
                Directory.CreateDirectory(directoryForLog);
        }
        public async void Cleanup()
        {
            await CombinePidFiles().ContinueWith((x) => DeleteOldLogs());
        }

        void CopyStream(Stream destination, Stream source)
        {
            const int BUFFER_SIZE = 4096;
            int count;
            byte[] buffer = new byte[BUFFER_SIZE];
            while ((count = source.Read(buffer, 0, buffer.Length)) > 0)
                destination.Write(buffer, 0, count);
        }
        protected async Task CombinePidFiles()
        {
            await Task.Run(() =>
            {
                try
                {
                    var random = new Random(pid);
                    var seconds = random.Next(0, 20);
                    Thread.Sleep(TimeSpan.FromSeconds(seconds));//wait a random amount of time to avoid conflict with another instance
                    var yesterday = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                    var files = Directory.GetFiles(logDirectory, $"log-{yesterday}*");
                    var destination = Path.Combine(logDirectory, $"log-{yesterday}.txt");
                    if (!File.Exists(destination))
                    {
                        var entries = new Dictionary<string, string>();
                        foreach (var file in files)
                        {
                            var all = File.ReadAllText(file);
                            var pattern = new Regex(@"^\S", RegexOptions.Multiline);
                            var lines = pattern.Split(all).Where(x => !string.IsNullOrEmpty(x));
                            foreach (var line in lines)
                            {
                                var dateStr = line.Substring(0, line.IndexOf('['));
                                var date = DateTime.ParseExact(dateStr, "h:mm:ss", CultureInfo.InvariantCulture).Ticks;
                                entries.Add(date +"-" + Guid.NewGuid().ToString(), line.Trim());
                            }

                        }
                        if (entries.Any())
                        {
                            using (var destinationFile = File.OpenWrite(destination))
                            {
                                using (var sw = new StreamWriter(destinationFile))
                                {
                                    foreach (var key in entries.Keys)
                                    {
                                        sw.Write(entries[key] + "\r\n");
                                    }
                                }
                            }
                        }
                        foreach (var file in files)
                        {
                            File.Delete(file);
                        }
                    }
                }
                catch (Exception)
                {
                    //do nothing.
                }
            });
        }

        protected async Task DeleteOldLogs()
        {
            await Task.Run(() =>
            {
                try
                {
                    var files = Directory.GetFiles(logDirectory, "log-*").AsEnumerable();
                    if (files.Count() > 7)
                    {
                        var toBeDeleted = files.OrderByDescending(x => x).Skip(7);
                        foreach (var file in toBeDeleted)
                            File.Delete(file);
                    }
                }
                catch (Exception ex)
                {
                    Record(LogLevel.Info, ex.Message, ex);
                }
            });
        }

        public override void Record(LogLevel level, string message, Exception ex = null, params object[] objectsToSerialize)
        {
            try
            {
                var today = DateTime.Now.ToString("yyyy-MM-dd");
                if (today != day)
                {
                    Cleanup();
                    var filename = FilenameFx();
                    //Combine pid files at the end of the day.
                    filePath = Path.Combine(logDirectory, filename);
                    day = today;
                }

                var logLine = Format(message, level, ex, objectsToSerialize);
                lock (hold)
                {
                    File.AppendAllText(filePath, logLine);
                }
            }
            catch (Exception)
            {
                //Nothing to do write to file isn't working
            }
        }

    }
}
