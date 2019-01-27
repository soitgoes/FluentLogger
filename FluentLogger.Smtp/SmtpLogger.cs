using System;
using System.Net.Mail;
using FluentLogger.Interfaces;

namespace FluentLogger.Smtp
{
    public class SmtpLogger : BaseLogger
    {
        private readonly SmtpClient client;
        private readonly string from;
        private readonly string to;
        private readonly object lockObj = new object();
        private readonly Action<string, string> sendAction;

        public SmtpLogger(SmtpClient client, string from, string to, LogLevel minLevel) : base(minLevel)
        {
            this.client = client;
            this.from = from;
            this.to = to;
        }
        /// <summary>
        /// Implement your own send action.  We'll compute the subject and body for you.
        /// </summary>
        /// <param name="sendAction"></param>
        /// <param name="minLevel"></param>
        public SmtpLogger(Action<string, string> sendAction, LogLevel minLevel): base(minLevel)
        {
            this.sendAction = sendAction;
        }

        public override void Record(LogLevel level, string message, Exception ex = null, params object[] objects)
        {
            lock (lockObj)
            {
                string subject = $"[{level.ToString()}] - {message}";
                string body = $"[{level.ToString()}] - {message}";
                if (sendAction != null)
                {
                    sendAction(subject, body);
                    return;
                }

                var mesg = new MailMessage(from, to);
                mesg.Subject = subject;
                mesg.Body = body;

                mesg.IsBodyHtml = true;
                if (ex != null)
                {
                    mesg.Body += "<br />" + ex.StackTrace;
                }

                //presently not supporting object serialization
                client.Send(mesg);
            }
        }
    }
}
