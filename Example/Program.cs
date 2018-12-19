using System;
using FluentLogger;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            LogFactory.Init(
                new DailyLogRoller(@"Logs", LogLevel.Trace),
                new SplitLogger(@"SplitLogs", LogLevel.Trace),
                new ConsoleLogger(LogLevel.Trace)
            );
            var logger = LogFactory.GetLogger();

            logger.Trace("Test Serialization", new { Name = "name" });
            logger.Info("Testing Info logs");
            logger.Warn("Warning, Danger Will Robinson");
            logger.Fatal("Fatal Error");
            logger.Error("Testing ERror");
            logger.Critical("Testing Critical");
            Console.ReadLine();
        }
    }
}
