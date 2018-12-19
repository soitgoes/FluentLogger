using System;
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
            Assert.AreEqual("\t\tTimeStamp : 10/05/2018 00:00:00 [System.DateTime]\n", raw);
        }
    }
}