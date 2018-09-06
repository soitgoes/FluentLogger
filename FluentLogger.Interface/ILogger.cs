using System;

namespace FluentLogger.Interfaces
{
    public interface ILog :IDisposable
    {
        void Record(LogLevel level, string message, Exception ex = null, params object[] objectsToSerialize);
        bool IsEnabled(LogLevel level);

        #region Methods 
        /* Trace */
        void Trace(string message);
        void Trace(string message, params object[] objects);
        /* Info */
        void Info(string message);
        void Info(string message, params object[] objects);
        /* Warnings */
        void Warn(string message);
        void Warn(string message, params object[] objects);
        void Warn(Exception ex);
        void Warn(string message, Exception ex);
        void Warn(string message, Exception ex, params object[] objects);
        /* Error */
        void Error(string message);
        void Error(string message, params object[] objects);
        void Error(string message, Exception ex);
        void Error(string message, Exception ex, params object[] objects);
        void Error(Exception ex);
        /* Critical */
        void Critical(string message);
        void Critical(string message, params object[] objects);
        void Critical(string message, Exception ex);
        void Critical(string message, Exception ex, params object[] objects);
        void Critical(Exception ex);

        /* Fatal */
        void Fatal(string message, Exception ex);
        void Fatal(Exception ex);
        void Fatal(string message, params object[] objects);
        void Fatal(string message, Exception ex, params object[] objects);
        #endregion
    }
}
