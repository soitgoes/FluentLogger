﻿using System;
using System.Threading;
using System.Net.Mail;
using FluentLogger;
using FluentLogger.Smtp;
using System.Threading.Tasks;
using System.IO;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var smtpClient = new SmtpClient("localhost");
            var userLogDir = Path.Combine("MaxFileSizeRoller", Environment.UserName);
            var headerFx = new Func<string>(() => "Version: 6.0.4");
            LogFactory.Init(
                //Add as many loggers as you like
                //Daily is deprecated, use MaximumFileSizeRoller instead
                //new DailyLogRoller(@"DailyLogRoller", LogLevel.Trace),
                new MaximumFileSizeRoller(userLogDir, LogLevel.Trace, headerFx, false, 2, 3, "log")
                , new ConsoleLogger(LogLevel.Trace)
            // ,new SmtpLogger(smtpClient, "errors@fluentlogger.com", "support@somewhere.com", LogLevel.Critical, "WFE01")
            );
            using (var logger = LogFactory.GetLogger())
            {
                int i = 0;
                while (i++ < 20)
                {
                    logger.Info("Primative", i);
                    try
                    {
                        throw new Exception("Thrown for your delight");
                    }
                    catch (Exception ex)
                    {
                        logger.Warn("Test Serialization", ex, new { Name = "name" }, null, "Just a plain ole string");
                        logger.Error(ex.Message, ex);
                    }

                    break;
                    //   Thread.Sleep(10);
                }

            }
            Console.ReadLine();


                
            
            
            
        }
    }
}
