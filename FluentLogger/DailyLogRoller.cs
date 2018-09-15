using System;
using System.IO;
using System.Linq;

namespace FluentLogger
{
    public class DailyLogRoller : BaseLogger
    {
        protected object hold = new object();
        protected static string logDirectory = Path.Combine(Environment.CurrentDirectory, "Logs");
        protected int counter = 0;
        public Func<string> FilenameFx = new Func<string>(() =>
        {
            return "log-" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
        });

        /// <summary>
        /// Initializes a new instance of the "FluentLogger.DailyLogRoller" class.
        /// Paramaters: string 
        /// </summary>
        public DailyLogRoller(string directoryForLog, LogLevel minLevel) :base(minLevel)
        {
            logDirectory = directoryForLog;
            if (!Directory.Exists(directoryForLog))
                Directory.CreateDirectory(directoryForLog);
        }

        /// <summary>
        /// Deletes the old logs.
        /// </summary>
        protected void DeleteOldLogs()
        {
            var files = Directory.GetFiles(logDirectory, "log-*").ToList();
            if (files.Count > 7)
            {
                var toBeDeleted = files.OrderByDescending(x => x).Skip(7);
                foreach (var file in toBeDeleted)
                    File.Delete(file);
            }
        }

        /// <summary>
        /// Record the specified level, message, ex and objectsToSerialize.
        /// Paramaters: LogLevel level, string message, Exception Ex  
        /// </summary>
        public override void Record(LogLevel level, string message, Exception ex = null, params object[] objectsToSerialize)
        {
            if (++counter > 1000)
            {
                DeleteOldLogs();
                counter = 0;
            }
            var filename = FilenameFx();
                
            var filePath = Path.Combine(logDirectory, filename);

            var logLine = Format(message, level, ex, objectsToSerialize);
            lock (hold)
            {
                File.AppendAllText(filePath, logLine);
            }
        }

    }
}
