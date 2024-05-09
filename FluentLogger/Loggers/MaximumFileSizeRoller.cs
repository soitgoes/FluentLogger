using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FluentLogger
{
    /// <summary>
    /// The maximum file size roller log roles to the next log when the specified filesize is reached or exceeded.
    /// This will put a hard upper limit on the file size of each log and 
    /// </summary>
    public class MaximumFileSizeRoller : BaseLogger, IDisposable
    {
        private static readonly Mutex rollMutex = new Mutex();
        private readonly string logDirectory;
        private readonly Func<string> logHeader;
        private readonly int maximumBytesPerFile;
        private readonly int numberOfFilesToKeep;
        private readonly string prefix;
        private readonly object hold = new object();
        private readonly string filename;
        private readonly string filePath;
        private BufferWriter bw;
        /// <summary>
        /// </summary>
        /// <param name="logDirectory"></param>
        /// <param name="minLevel">Any message beneath the threshold is ignored</param>
        /// <param name="logHeader">Message to start each log file after roll</param>
        /// <param name="strict">throws Exception if Directory doesn't exists or the user doesn't have permissions</param>
        /// <param name="maximumMgPerFile">Size in Megabytes of each log file.  Defaults to 5</param>
        /// <param name="numberOfFilesToKeep">The number of log files to retain.  Defaults to 5</param>
        /// <param name="fileNamePrefix">The filename prefix for the log file.  Defaults to log-[<pid>]</param>
        public MaximumFileSizeRoller(string logDirectory, LogLevel minLevel, Func<string> logHeader, bool strict = false, int maximumMgPerFile = 5, int numberOfFilesToKeep = 5, string filenamePrefix = null) : base(minLevel)
        {
            if (strict)
                AssertDirectoryExistsAndWritable(logDirectory);
            else
                EnsureDirectoryExists(logDirectory);
            this.logDirectory = logDirectory;
            this.logHeader = logHeader;
            this.maximumBytesPerFile = maximumMgPerFile * 1024 * 1024;
            this.numberOfFilesToKeep = numberOfFilesToKeep;
            this.prefix = filenamePrefix ?? $"log-{pid}";
            this.filename = $"{prefix}.current.txt";
            this.filePath = Path.Combine(logDirectory, filename);

            InitStream();
            RecordHeader();
        }

        public void InitStream()
        {
            this.bw = new BufferWriter(this.filePath);

        }

        public override void Dispose()
        {
            this.bw?.Flush();
            this.bw?.Dispose();
        }
        public override void Flush()
        {
            this.bw?.Flush();
        }
        /// <summary>
        /// If directory doesn't exist attempt to create and fail silent if we can't 
        /// </summary>
        /// <param name="dir"></param>
        protected void EnsureDirectoryExists(string dir)
        {
            if (Directory.Exists(dir)) return;
            try
            {
                Directory.CreateDirectory(dir);
            }
            catch (Exception)
            {
                //with strict false we fail silent
            }
        }

        protected void AssertDirectoryExistsAndWritable(string dir)
        {
            if (!Directory.Exists(dir)) throw new DirectoryNotFoundException("MaximumFileSizeRoller: Directory not found:" + dir);
            if (!IsDirectoryWritable(dir)) throw new UnauthorizedAccessException("User does not have permission to write to configured directory:" + dir);
        }
        private bool IsDirectoryWritable(string dirPath, bool throwIfFails = false)
        {
            try
            {
                using (FileStream fs = File.Create(
                    Path.Combine(
                        dirPath,
                        Path.GetRandomFileName()
                    ),
                    1,
                    FileOptions.DeleteOnClose)
                )
                { }
                return true;
            }
            catch
            {
                if (throwIfFails)
                    throw;
                else
                    return false;
            }
        }
        /// <summary>
        /// Shift files and move current to 1
        /// </summary>
        private void RollLogs()
        {
            try
            {
                rollMutex.WaitOne(1000);
                var files = Directory.GetFiles(logDirectory, $"{prefix}.*.txt");
                var dict = new Dictionary<int, string>();
                foreach (var file in files)
                {
                    if (file.Contains("current")) continue;
                    var parts = file.Split('.');
                    int number = int.Parse(parts[parts.Length - 2]);
                    dict.Add(number, new FileInfo(file).Name);
                }
                this.Dispose();
                foreach (var number in dict.Keys.OrderByDescending(x => x))
                {
                    if (number >= numberOfFilesToKeep)
                    {
                        var pathToDelete = Path.Combine(logDirectory, dict[number]);
                        if (File.Exists(pathToDelete))
                            File.Delete(pathToDelete);
                    }
                    var src = Path.Combine(logDirectory, dict[number]);
                    var tar = Path.Combine(logDirectory, $"{prefix}.{number + 1}.txt");

                    if (File.Exists(src))
                        File.Move(src, tar);
                }

                var target = Path.Combine(logDirectory, $"{prefix}.1.txt");

                File.Move(this.filePath, target);
                InitStream();
                RecordHeader();

            }
            finally
            {
                rollMutex.ReleaseMutex();
            }

            
        }
        public void RecordHeader()
        {
            this.bw.Append($"-------{logHeader()}-------" + Environment.NewLine);
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
                bw.Append(logLine);
            }
            catch (Exception e)
            {
                //Nothing to do write to file isn't working
            }
        }
    }
}
