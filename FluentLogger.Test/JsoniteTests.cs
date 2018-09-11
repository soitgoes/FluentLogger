using NUnit.Framework;
namespace Jsonite.Tests
{
    [TestFixture]
    public class JsoniteTests
    {
        [Test]
        public void ShouldSerializeObjectTypes()
        {
            //Arrange & Act
            var raw = Jsonite.Json.Serialize(new { Name = "Test" });

            Assert.AreEqual("{ name='Test' }", raw);
        }
    }
}
