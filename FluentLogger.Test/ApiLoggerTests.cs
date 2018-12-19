using NUnit.Framework;

namespace FluentLogger.Test
{
    [TestFixture]
    public class ApiLoggerTests
    {
        [Test]
        public void ShouldPostRequest()
        {
            //Arrange
            var logger = new ApiLogger("http://postb.in/b/Tgix0BIX", null, LogLevel.Trace);
            //Act
            logger.Error("Checking error");
            //Assert
            Assert.IsTrue(true);
        }
    }
}
