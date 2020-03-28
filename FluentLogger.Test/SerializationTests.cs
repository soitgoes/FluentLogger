using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FluentLogger.Test
{
    [TestFixture]
    public class SerializationTests
    {
        [Test]
        public void ShouldSerializeDateTime()
        {
            var raw = BaseLogger.Serialize(new {TimeStamp = DateTime.Parse("2018-10-05")});
            Assert.AreEqual("\t\tTimeStamp : 10/5/2018 12:00:00 AM [System.DateTime]\r\n", raw);
        }


        [Test]
        public void ShouldSerializeIEnumerable()
        {
            IEnumerable<string> strs = new string[] { "hi", "There" };
            var raw = BaseLogger.Serialize(strs);
            Assert.AreEqual("\t\t\"hi\" :  [System.String]\r\n\t\t\"There\" :  [System.String]\r\n", raw);
        }

        [Test]
        public  void ShouldEnumerateStackTracesWithMoreThanOneError()
        {
            var bl = new ConsoleLogger(LogLevel.Info);

            MultipleErrors().ContinueWith(c =>
            {
                var str = BaseLogger.Format("", LogLevel.Info, c.Exception, null);
                Assert.IsTrue(str.Contains("Exception 1"));
            }).Wait();    
        }

        public async Task MultipleErrors()
        {
            await Task.WhenAll(Task.Run(() => throw new Exception("Exception 1")),
                Task.Run(() => throw new Exception("Exception 2")));
        }
    }
}