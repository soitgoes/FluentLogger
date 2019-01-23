﻿using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;

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

        protected internal virtual void PostData(object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            //var request = (HttpWebRequest)WebRequest.Create(postUrl);
            //request.Method = "POST";
            ////request.Credentials = this.credentials;
            //using (var stream = request.GetRequestStream())
            //{
            //    using (var sr = new StreamWriter(stream))
            //    {
            //        sr.Write(json);
            //        var response = request.GetResponse();

            //    }
            //}

            var response =  PostRaw(json);

        }
        protected string PostRaw( string rawBody)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(postUrl);
                request.Method = "POST";
                request.ContentType = "application/json";
                using (var stream = request.GetRequestStream())
                {
                    using (var sw = new StreamWriter(stream))
                    {
                        sw.Write(rawBody);
                    }
                }

                using (var stream = request.GetResponse().GetResponseStream())
                {
                    using (var sr = new StreamReader(stream))
                    {
                        var str = sr.ReadToEnd().Replace("\\\"", "\"");
                        return str;
                    }
                }

            }
            catch (Exception ex)
            {
               // throw ex;
            }
            return null;
        }


        public override void Record(LogLevel level, string message, Exception ex = null, params object[] objectsToSerialize)
        {
            try
            {
                if (string.IsNullOrEmpty(message))
                    message = ex.Message;  
                 PostData(new {id=Guid.NewGuid().ToString(), machineName=Environment.MachineName, level, message, exception=ex?.StackTrace, objectsToSerialize });
            }
            catch (Exception localEx)
            {
                Console.WriteLine(localEx.Message);
            }
        }
    }
}
