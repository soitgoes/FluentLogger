using Jsonite;
using System;
using System.IO;
using System.Net;

namespace FluentLogger
{
    public class ApiLogger : BaseLogger
    {
        private readonly string postUrl;
        private readonly NetworkCredential credentials;

        public ApiLogger(string postUrl, NetworkCredential credentials, LogLevel minLevel) : base(minLevel)
        {
            this.postUrl = postUrl;
            this.credentials = credentials;
        }

        public virtual void PostData(object obj)
        {
            var json = Json.Serialize(obj);
            var request = (HttpWebRequest)WebRequest.Create(postUrl);
            request.Method = "POST";
            request.Credentials = this.credentials;
            using (var stream = request.GetRequestStream())
            {
                using (var sr = new StreamWriter(stream))
                {
                    sr.Write(json);
                }
            }
            request.GetResponse();
        }


        public override void Record(LogLevel level, string message, Exception ex = null, params object[] objectsToSerialize)
        {
            try
            {
                PostData(new { level, message, ex, objectsToSerialize });
            }
            catch (Exception localEx)
            {
                Console.WriteLine(localEx.Message);
            }
        }
    }
}
