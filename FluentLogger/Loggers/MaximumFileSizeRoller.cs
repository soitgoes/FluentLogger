using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FluentLogger
{
    /// <summary>
    /// The maximum file size roller log roles to the next log when the specified filesize is reached or exceeded.
    /// This will put a hard upper limit on the file size of each log and 
    /// </summary>
    public class MaximumFileSizeRoller : BaseLogger
    {
        private readonly string logDirectory;
        private readonly int maximumBytesPerFile;
        private readonly int numberOfFilesToKeep;
        private readonly object hold = new object();
        private readonly string filename;
        private readonly string filePath;

        /// <summary>
        /// </summary>
        /// <param name="logDirectory"></param>
        /// <param name="minLevel">Any message beneath the threshold is ignored</param>
        /// <param name="maximumMgPerFile">Size in Megabytes of each log file.  Defaults to 5</param>
        /// <param name="numberOfFilesToKeep">The number of log files to retain.  Defaults to 5</param>
        public MaximumFileSizeRoller(string logDirectory, LogLevel minLevel, int maximumMgPerFile=5, int numberOfFilesToKeep=5) : base(minLevel)
        {
            this.logDirectory = logDirectory;
            this.maximumBytesPerFile = maximumMgPerFile * 1024 * 1024;
            this.numberOfFilesToKeep = numberOfFilesToKeep;
            this.filename = $"log-[{pid}].current.txt";
            this.filePath = Path.Combine(logDirectory, filename);
        }
        
        /// <summary>
        /// Shift files and move current to 0
        /// </summary>
        private void RollLogs()
        {
            var files = Directory.GetFiles(logDirectory, $"log-[{pid}].*.txt");
            var dict = new Dictionary<int, string>();
            foreach (var file in files)
            {
                if (file.Contains("current")) continue;
                var parts = file.Split('.');
                int number = int.Parse(parts[parts.Length - 2]);
                dict.Add(number, file);
            }
            
            foreach (var number in dict.Keys.OrderByDescending(x => x))
            {
                if (number >= numberOfFilesToKeep)
                {
                    var pathToDelete = Path.Combine(logDirectory, dict[number]);
                    lock (hold)
                    {
                        if (File.Exists(pathToDelete))
                            File.Delete(pathToDelete);
                    }
                }
                var src =    Path.Combine(logDirectory, dict[number]);
                var target = Path.Combine(logDirectory,  $"log-[{pid}].{number+1}.txt");
                lock (hold)
                {
                    if (File.Exists(src))
                        File.Move(src, target);
                }
            }
            lock (hold)
            {
                var target = Path.Combine(logDirectory, $"log-[{pid}].1.txt");
                File.Move(this.filePath, target);
            }
        }
        
        public override void Record(LogLevel level, string message, Exception ex = null, params object[] objectsToSerialize)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var fileInfo = new FileInfo(filePath);
                    if (fileInfo.Length > maximumBytesPerFile)
                        RollLogs();
                }

                var logLine = Format(message, level, ex, objectsToSerialize);
                lock (hold)
                {
                    File.AppendAllText(filePath, logLine);
                }
            }
            catch (Exception ex1)
            {
                //Nothing to do write to file isn't working
            }
        }
    }
}
