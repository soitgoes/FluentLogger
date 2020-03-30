using FluentLogger.Interfaces;
using System;
using System.Collections;
using System.Diagnostics;
using System.Linq.Expressions;



namespace FluentLogger
{
    public abstract class BaseLogger : ILog
    {
        protected static int pid = Process.GetCurrentProcess().Id;
        public LogLevel MinLevel
        {
            get { return minLevel; }
            set { minLevel = value; }
        }

        protected LogLevel minLevel;

        public static Func<string, LogLevel, Exception, object[], string> Format =
            new Func<string, LogLevel, Exception, object[], string>((mesg, logLevel, ex, objects) =>
         {
             var logLine = DateTime.UtcNow.ToLongTimeString() + "[" + logLevel.ToString().ToUpper() + "] " + mesg + Environment.NewLine;
            if (ex != null)
            {
                logLine += "\t\t\t" + ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine;
                if (ex is AggregateException)
                {
                    var aggEx = ex as AggregateException;
                    string lines = "";
                    foreach (var innerEx in aggEx.InnerExceptions)
                    {
                        lines += Format(innerEx.Message, logLevel, innerEx, null);
                    }
                    return lines;
                }
            }
            if (objects != null)
            {
                foreach (var obj in objects)
                    logLine += Serialize(obj) + Environment.NewLine;
            }
            
             return logLine;
         });
        
        public static string Serialize(object obj)
        {
            try
            {
                if (obj == null) return "null";
                var result = "";

                if (obj is IEnumerable)
                {
                    foreach (var item in (IEnumerable)obj)
                    {
                        result += BuildString(item);
                    }
                }
                else
                {
                    result += BuildString(obj);
                }
                return result;
            }
            catch (Exception ex)
            {
                return "Error attempting to serialize object: " + ex.Message;
            }
        }

        private static string BuildString(object obj)
        {
            string result = "";
            var t = obj.GetType();
            if (obj is string)
            {
                result += "\t\t\"" + obj + "\" : " + " [System.String]" +
                          Environment.NewLine;
            }
            else
            {
                foreach (var prop in t.GetProperties())
                {
                    result += "\t\t" + prop.Name + " : " + prop.GetValue(obj) + " [" + prop.PropertyType.ToString() + "]" +
                              Environment.NewLine;
                }
            }
            return result;
        }

        public BaseLogger(LogLevel minLevel)
        {
            this.minLevel = minLevel;
        }

        public abstract void Record(LogLevel level, string message, Exception ex = null, params object[] objectsToSerialize);

        /// <summary>
        /// This records the log entry unless it does not meet the minimum specified log level.
        /// </summary>
        /// <param name="level">Identifies what type of log entry is being captured.</param>
        /// <param name="message">Contains the details of the log entry.</param>
        /// <param name="ex">Stack trace is recorded.</param>
        /// <param name="objectsToSerialize">Any additional data objects you'd like to persist to the log.</param>
        protected virtual void Filter(LogLevel level, string message, Exception ex = null, params object[] objectsToSerialize)
        {
            if (level < minLevel) return;
            Record(level, message, ex, objectsToSerialize);
        }

        public void Error(string message, Exception ex)
        {
            Filter(LogLevel.Error, message, ex);
        }

        public void Error(Exception ex)
        {
            Filter(LogLevel.Error, null, ex);
        }
        public void Fatal(string message, Exception ex)
        {
            Filter(LogLevel.Fatal, message, ex);
        }
        public void Fatal(Exception ex)
        {
            Filter(LogLevel.Fatal, null, ex);
        }

        public void Info(string message)
        {
            Filter(LogLevel.Info, message);
        }

        public void Trace(string message)
        {
            Filter(LogLevel.Trace, message);
        }
        public void Warn(Exception ex)
        {
            Filter(LogLevel.Warn, null, ex);
        }
        public void Warn(string message)
        {
            Filter(LogLevel.Warn, message);
        }

        public void Warn(string message, Exception ex)
        {
            Filter(LogLevel.Warn, message, ex);
        }

        public bool IsEnabled(LogLevel level)
        {
            return (level >= minLevel);
        }

        public virtual void Dispose()
        {
        }



        public void Trace(string message, params object[] objects)
        {
            Filter(LogLevel.Trace, message, null, objects);
        }

        public void Info(string message, params object[] objects)
        {
            Filter(LogLevel.Info, message, null, objects);
        }

        public void Warn(string message, params object[] objects)
        {
            Filter(LogLevel.Warn, message, null, objects);
        }

        public void Warn(string message, Exception ex, params object[] objects)
        {
            Filter(LogLevel.Warn, message, ex, objects);
        }

        public void Error(string message)
        {
            Filter(LogLevel.Error, message);
        }

        public void Error(string message, params object[] objects)
        {
            Filter(LogLevel.Error, message, null, objects);
        }

        public void Error(string message, Exception ex, params object[] objects)
        {
            Filter(LogLevel.Error, message, ex, objects);
        }

        public void Critical(string message)
        {
            Filter(LogLevel.Critical, message);
        }

        public void Critical(string message, params object[] objects)
        {
            Filter(LogLevel.Critical, message, null, objects);
        }



        public void Critical(string message, Exception ex)
        {
            Filter(LogLevel.Critical, message, ex);
        }

        public void Critical(string message, Exception ex, params object[] objects)
        {
            Filter(LogLevel.Critical, message, ex, objects);
        }

        public void Critical(Exception ex)
        {
            Filter(LogLevel.Critical, null, ex);
        }

        public void Fatal(string message, params object[] objects)
        {
            Filter(LogLevel.Fatal, message, null, objects);
        }

        public void Fatal(string message, Exception ex, params object[] objects)
        {
            Filter(LogLevel.Fatal, message, ex, objects);
        }
    }
}
