using FluentLogger.Interfaces;
using System;
using Jsonite;

namespace FluentLogger
{
    public abstract class BaseLogger : ILog
    {
        protected readonly LogLevel minLevel;
        public Func<string, LogLevel, Exception, object[], string> Format { get; set; } = new Func<string, LogLevel, Exception, object[], string>((mesg, logLevel, ex, objects) =>
         {
             var logLine = DateTime.Now.ToString("hh:mm:ss") + "[" + logLevel.ToString().ToUpper() + "] " + mesg + Environment.NewLine;
             if (ex != null)
             {
                 logLine += "\t\t\t" + ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine;
                 if (ex is AggregateException)
                 {
                     var aggEx = ex as AggregateException;
                     logLine += aggEx.Flatten().Message;
                 }
             }
             foreach (var obj in objects)
                 logLine += Json.Serialize(obj) + Environment.NewLine;
             return logLine;
         });
        public BaseLogger(LogLevel minLevel)
        {
            this.minLevel = minLevel;
        }

        public abstract void Record(LogLevel level, string message, Exception ex = null, params object[] objectsToSerialize);

        /// <summary>
        /// Filter the specified level, message, ex and objectsToSerialize.
        /// Parameters: LogLevel level, string message, Exception ex = null, params object[] objectsToSerialize
        /// </summary>
        private void Filter(LogLevel level, string message, Exception ex = null, params object[] objectsToSerialize)
        {
            if (level < minLevel) return;
            Record(level, message, ex, objectsToSerialize);
        }

        /// <summary>
        /// Error the specified message and ex.
        /// Parameters: string message, Exception ex
        /// </summary>
        public void Error(string message, Exception ex)
        {
            Filter(LogLevel.Error, message, ex);
        }

        /// <summary>
        /// Error the specified ex.
        /// Parameters: Exception ex
        /// </summary>
        public void Error(Exception ex)
        {
            Filter(LogLevel.Error, null, ex);
        }

        /// <summary>
        /// Fatal the specified message and ex.
        /// Parameters: string message, Exception ex
        /// </summary>
        public void Fatal(string message, Exception ex)
        {
            Filter(LogLevel.Fatal, message, ex);
        }

        /// <summary>
        /// Fatal the specified ex.
        /// Parameters: Exception ex
        /// </summary>
        public void Fatal(Exception ex)
        {
            Filter(LogLevel.Fatal, null, ex);
        }

        /// <summary>
        /// Info the specified message.
        /// Parameters: string message
        /// </summary>
        public void Info(string message)
        {
            Filter(LogLevel.Info, message);
        }

        /// <summary>
        /// Trace the specified message.
        /// Parameters: string messages
        /// </summary>
        public void Trace(string message)
        {
            Filter(LogLevel.Trace, message);
        }

        /// <summary>
        /// Warn the specified ex.
        /// Parameters: Exception ex
        /// </summary>
        public void Warn(Exception ex)
        {
            Filter(LogLevel.Warn, null, ex);
        }

        /// <summary>
        /// Warn the specified message.
        /// Parameters: string message
        /// </summary>
        public void Warn(string message)
        {
            Filter(LogLevel.Warn, message);
        }

        /// <summary>
        /// Warn the specified message.
        /// Parameters: string message, exception ex 
        /// </summary>
        public void Warn(string message, Exception ex)
        {
            Filter(LogLevel.Warn, message, ex);
        }

        /// <summary>
        /// Ises the enabled.
        /// Parameters: LogLevel level
        /// Returns: level
        public bool IsEnabled(LogLevel level)
        {
            return (level >= minLevel);
        }

        /// <summary>
        /// Releases all resource used by the FluentLogger.BaseLogger object.
        /// </summary>
        public virtual void Dispose()
        {
        }


        /// <summary>
        /// Trace the specified message and objects.
        /// Parameters: string message, params object[] objects
        /// </summary>
        public void Trace(string message, params object[] objects)
        {
            Filter(LogLevel.Trace, message, null, objects);
        }

        /// <summary>
        /// Info the specified message and objects.
        /// Parameters: string message, Exception ex params object[] objects
        /// </summary>
        public void Info(string message, params object[] objects)
        {
            Filter(LogLevel.Info, message, null, objects);
        }

        /// <summary>
        /// Warn the specified message and objects.
        /// Parameters: string message, Exception ex
        /// </summary>
        public void Warn(string message, params object[] objects)
        {
            Filter(LogLevel.Warn, message, null, objects);
        }

        /// <summary>
        /// Warn the specified message, ex and objects.
        /// Parameters: string message, Exception ex, params object[] objects
        /// </summary>
        public void Warn(string message, Exception ex, params object[] objects)
        {
            Filter(LogLevel.Warn, message, ex, objects);
        }

        /// <summary>
        /// Filters the LogLevel message
        /// Parameters: string message
        /// </summary>
        public void Error(string message)
        {
            Filter(LogLevel.Error, message);
        }

        /// <summary>
        /// Error the specified message and objects.
        /// Parameters: string message, params object[] objects
        /// </summary>
        public void Error(string message, params object[] objects)
        {
            Filter(LogLevel.Error, message, null, objects);
        }

        /// <summary>
        /// Error the specified message, ex and objects.
        /// Parameters: string message, Exception ex, params object[] objects
        /// </summary>
        public void Error(string message, Exception ex, params object[] objects)
        {
            Filter(LogLevel.Error, message, ex, objects);
        }

        /// <summary>
        /// Critical the specified message.
        /// Parameters: string message
        /// </summary>
        public void Critical(string message)
        {
            Filter(LogLevel.Critical, message);
        }

        /// <summary>
        /// Critical the specified message and objects.
        /// Parameters: string message, params object[] objects
        /// </summary>
        public void Critical(string message, params object[] objects)
        {
            Filter(LogLevel.Critical, message, null, objects);
        }

        /// <summary>
        /// Critical the specified message and ex.
        /// Parameters: string message, Exception ex
        /// </summary>
        public void Critical(string message, Exception ex)
        {
            Filter(LogLevel.Critical, message, ex);
        }

        /// <summary>
        /// Critical the specified message and ex.
        /// Parameters: string message, Exception ex, params object[] objects 
        /// </summary>
        public void Critical(string message, Exception ex, params object[] objects)
        {
            Filter(LogLevel.Critical, message, ex, objects);
        }

        /// <summary>
        /// Critical the specified ex.
        /// Parameters: Exception ex
        /// </summary>
        public void Critical(Exception ex)
        {
            Filter(LogLevel.Critical, null, ex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="objects"></param>
        public void Fatal(string message, params object[] objects)
        {
            Filter(LogLevel.Fatal, message, null, objects);
        }

        /// <summary>
        /// Fatal the specified message, ex and objects
        /// Parameters: string message, Exception ex, params object[] objects
        /// </summary>
        public void Fatal(string message, Exception ex, params object[] objects)
        {
            Filter(LogLevel.Fatal, message, ex, objects);
        }
    }
}
