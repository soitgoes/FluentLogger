using System;
using System.Collections.Generic;
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
            Assert.AreEqual("\t\thi :  [System.String]\r\n\t\tThere :  [System.String]\r\n", raw);
        }
    }
}