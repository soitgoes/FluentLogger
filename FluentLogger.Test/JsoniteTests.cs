using Xunit;

namespace Jsonite.Tests
{
    public class JsoniteTests
    {
        [Fact]
        public void ShouldSerializeObjectTypes()
        {
            //Arrange & Act
           var raw = Jsonite.Json.Serialize(new { Name = "Test" });

            Assert.Equal("{ name='Test' }", raw);
        }
    }
}
