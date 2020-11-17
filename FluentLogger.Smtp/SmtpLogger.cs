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
        private readonly string sourceSite;

        private SmtpLogger(SmtpClient client, string from, string to, LogLevel minLevel) : base(minLevel)
        {
            this.client = client;
            this.from = from;
            this.to = to;
        }

        /// <summary>
        /// Be cognisant that using this logger should only be used on rarish messages.  
        /// Using it on anything that happens more often could compromise sendability of whatever you are using for email
        /// </summary>
        /// <param name="client">The Smtp client used to send the message with error.  Should have auth configured if necessary</param>
        /// <param name="from">The email address your sending from</param>
        /// <param name="to">The email address your sending to</param>
        /// <param name="minLevel">The minimum log level threshold</param>
        /// <param name="sourceSite">If you have more than one server/instance it's nice to know which site reported this message.</param>
        public SmtpLogger(SmtpClient client, string from, string to, LogLevel minLevel, string sourceSite) : this(client, from, to, minLevel)
        {
            this.sourceSite = sourceSite;
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
                message = string.IsNullOrEmpty(message) ? ex?.Message : message; 
                string subject = $"[{level.ToString()}] {Environment.MachineName}/{sourceSite} - {message}";
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
