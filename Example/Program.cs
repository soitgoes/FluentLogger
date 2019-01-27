using System;
using System.Net.Mail;
using FluentLogger;
using FluentLogger.Smtp;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var smtpClient = new SmtpClient("localhost");
            LogFactory.Init(
                //Add as many loggers as you like
                new DailyLogRoller(@"c:\Temp\Logs", LogLevel.Trace),
                new ConsoleLogger(LogLevel.Trace),
                new SmtpLogger(smtpClient, "errors@fluentlogger.com", "support@somewhere.com", LogLevel.Critical)
            );
            //LogFactory.Init(new ConsoleLogger(LogLevel.Fatal));
            var logger = LogFactory.GetLogger();
            

            logger.Trace("Test Serialization", new { Name = "name" });
            logger.Fatal("Fatal Error");
            Console.ReadLine();
        }
    }
}
