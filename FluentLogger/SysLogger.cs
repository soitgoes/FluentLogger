using SyslogNet.Client;
using SyslogNet.Client.Serialization;
using SyslogNet.Client.Transport;
using System;



namespace FluentLogger
{
    public class SysLogger : BaseLogger
    {
        private readonly SyslogUdpSender client;
        private readonly string appName;

        public SysLogger(LogLevel minLevel, string appName, string host, int port) : base(minLevel)
        {
            client = new SyslogUdpSender(host, port);
            this.appName = appName;
        }
        public override void Record(LogLevel level, string message, Exception ex = null, params object[] objectsToSerialize)
        {
            if (ex != null)
            {
                message += "\n\t\t\t" + ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine;
            }
            if (objectsToSerialize != null)
            {
                foreach (var obj in objectsToSerialize)
                    message += Serialize(obj) + Environment.NewLine;
            }
            var mesg = new SyslogMessage(ConvertLevel(level), appName, message);
            var serializer = new SyslogRfc5424MessageSerializer();
            serializer.Serialize(mesg);            
            client.Send(mesg, serializer);
        }

        private Severity ConvertLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Critical:
                    return Severity.Critical;
                case LogLevel.Error:
                    return Severity.Error;
                case LogLevel.Fatal:
                    return Severity.Emergency;
                case LogLevel.Info:
                    return Severity.Informational;
                case LogLevel.Trace:
                    return Severity.Notice;
                case LogLevel.Warn:
                    return Severity.Warning;
            }
            return Severity.Debug;
        }
    }
}
