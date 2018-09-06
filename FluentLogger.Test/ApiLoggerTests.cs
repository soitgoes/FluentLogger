using Xunit;

namespace FluentLogger.Test
{
    public class ApiLoggerTests
    {
        [Fact]
        public void ShouldPostRequest()
        {
            //Arrange
            var logger = new ApiLogger("http://postb.in/b/Tgix0BIX", null, LogLevel.Trace);
            //Act
            logger.Error("Checking error");
            //Assert
            Assert.True(true);
        }
    }
}
