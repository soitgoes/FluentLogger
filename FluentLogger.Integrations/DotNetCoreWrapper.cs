
using System;
using Microsoft.Extensions.Logging;

namespace FluentLogger.Integrations
{
    public class DotNetCoreWrapper : ILogger
    {
        private readonly Interfaces.ILog logger;

        public DotNetCoreWrapper(Interfaces.ILog logger){
            
            this.logger = logger;
        }

        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
        {
            return logger.IsEnabled(LevelMapUp(logLevel));
        }
        

        IDisposable ILogger.BeginScope<TState>(TState state)
        {
            return logger;
        }

        void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            logger.Record(LevelMapUp(logLevel), formatter(state, exception));
        }
        private FluentLogger.LogLevel LevelMapUp(Microsoft.Extensions.Logging.LogLevel level)
        {
            switch (level)
            {
                case Microsoft.Extensions.Logging.LogLevel.Critical:
                    return LogLevel.Fatal;
                case Microsoft.Extensions.Logging.LogLevel.Error:
                    return LogLevel.Error;
                case Microsoft.Extensions.Logging.LogLevel.Trace:
                case Microsoft.Extensions.Logging.LogLevel.Debug:
                    return LogLevel.Trace;
                case Microsoft.Extensions.Logging.LogLevel.Warning:
                    return LogLevel.Warn;
                case Microsoft.Extensions.Logging.LogLevel.Information:
                    return LogLevel.Info;
                default:
                    return LogLevel.Trace;
            }
        }

        void ILogger.Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            throw new NotImplementedException();
        }
    }
}
