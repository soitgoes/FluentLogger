using System;
using System.Net.Mail;

namespace FluentLogger.Smtp
{
    public class SmtpLogger : BaseLogger
    {
        private readonly SmtpClient client;
        private readonly string from;
        private readonly string to;
       
        public SmtpLogger(SmtpClient client, string from, string to, LogLevel minLevel) : base(minLevel)
        {
            this.client = client;
            this.from = from;
            this.to = to;
        }

        public override void Record(LogLevel level, string message, Exception ex = null, params object[] objects)
        {
            var mesg = new MailMessage(from, to);
            mesg.Subject = $"[{level.ToString()}] -{message}";
            mesg.Body = $"[{level.ToString()}] -{message}";
            mesg.IsBodyHtml = true;
            if (ex != null)
            {
                mesg.Body += "<br />" + ex.StackTrace;
            }
            //presently not supporting object serialization
            client.SendAsync(mesg, null);
        }
    }
}
