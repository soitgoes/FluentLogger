using System;

namespace FluentLogger
{
    public class ConsoleLogger : BaseLogger{
        public ConsoleLogger(LogLevel minLevel = LogLevel.Trace) : base(minLevel)
        {
        }


        public override void Record(LogLevel level, string message, Exception ex = null, params object[] objectsToSerialize)
        {
            Console.WriteLine($"[{level.ToString().ToUpper()}] - {message}" + (ex ==  null ? "" : "-" + ex.StackTrace));
        }
    }
}
