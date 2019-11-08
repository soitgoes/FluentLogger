using System;
using System.Threading;
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
                //Daily is deprecated, use MaximumFileSizeRoller instead
                //new DailyLogRoller(@"C:\Users\marti\AppData\Roaming\911Cellular", LogLevel.Trace),
                new MaximumFileSizeRoller(@"C:\Users\marti\AppData\Roaming\911Cellular", LogLevel.Trace, 2, 3 )
                ,new ConsoleLogger(LogLevel.Trace)
                ,new SmtpLogger(smtpClient, "errors@fluentlogger.com", "support@somewhere.com", LogLevel.Critical)
            );
            //LogFactory.Init(new ConsoleLogger(LogLevel.Fatal));
            var logger = LogFactory.GetLogger();
            
            
            while (true)
            {
                logger.Trace("Test Serialization", new { Name = "name" });
                logger.Fatal("Fatal Error");
                //Thread.Sleep(100);
            }
            
        }
    }
}
