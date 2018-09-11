using System;
using FluentLogger;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            LogFactory.Init(
                new DailyLogRoller(@"c:\Temp\Logs", LogLevel.Trace),
                new ConsoleLogger(LogLevel.Trace)
            );
            var logger = LogFactory.GetLogger();

            logger.Trace("Test Serialization", new { Name = "name" });
            Console.ReadLine();
        }
    }
}
