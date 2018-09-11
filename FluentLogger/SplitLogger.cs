using System;

namespace FluentLogger
{
    public class SplitLogger : DailyLogRoller
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FluentLogger.SplitLogger"/> class.
        /// The Split Logger creates a different file for each Log Level for every day. 
        /// </summary>
        /// <param name="directory">Destination directory for the log</param>
        /// <param name="minLevel">Minimum Log level required in order to write the log message</param>
        public SplitLogger(string directory, LogLevel minLevel): base(directory, minLevel)
        { 
        }

        /// <summary>
        /// Record the specified level, message, ex and objectsToSerialize.
        /// </summary>
        /// <param name="level">Level.</param>
        /// <param name="message">Message.</param>
        /// <param name="ex">Ex.</param>
        /// <param name="objectsToSerialize">Objects to serialize.</param>
        public override void Record(LogLevel level, string message, Exception ex = null, params object[] objectsToSerialize)
        {
            FilenameFx = new Func<string>(() =>
            {
                return "log-" + DateTime.Now.ToString("yyyy-MM-dd") + "-" + level.ToString() + ".txt";
            });
            base.Record(level, message, ex, objectsToSerialize);
        }

    }
}
