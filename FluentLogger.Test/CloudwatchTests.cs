using NUnit.Framework;
using FluentLogger;
using Amazon;
using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using FluentLogger.CloudWatch;

namespace FluentLogger.Test
{
    [TestFixture]
    public class CloudwatchTests
    {
        private readonly CloudWatch.CloudWatchLogger cloudWatchLogger;
        /*
         * credentials here
         */
        private readonly string accessKey = "";
        private readonly string secretKey = "";
        // -------------------
        private readonly RegionEndpoint region;
        private readonly string groupName; // this group must already exist in cloudwatch
        private readonly string streamName; // this stream must already exist in cloudwatch

        public CloudwatchTests()
        {
            region = RegionEndpoint.USEast2;
            groupName = ""; // modify to existing group
            streamName = ""; // modify to existing stream

            cloudWatchLogger = new CloudWatch.CloudWatchLogger(LogLevel.Info, accessKey, secretKey, region, groupName, streamName);
        }

        [Test]
        public async Task CanDescribeLogStreams()
        {
            if (cloudWatchLogger == null) Assert.False(true);

            var logStream = await cloudWatchLogger.GetLogStream();
            Assert.NotNull(logStream);
        }

        /*
         * also verify it worked by checking cloudwatch
         */ 
        [Test]
        public async Task TestPostingLog()
        {
            if (cloudWatchLogger == null) Assert.False(true);

            var stream = await cloudWatchLogger.GetLogStream();
            var result = await cloudWatchLogger.PutLogs(new List<InputLogEvent>()
            {
                new InputLogEvent() { Message = "Test Log to CloudWatch", Timestamp = DateTime.UtcNow }
            },
            stream.UploadSequenceToken);

            Assert.AreEqual("OK", result.HttpStatusCode.ToString());
        }

        [Test]
        public async Task TestLogFactory()
        {
            /*LogFactory.Init();
            LogFactory.AddLogger(
                                    new CloudWatchLogger(LogLevel.Info,
                                    accessKey,
                                    secretKey,
                                    Amazon.RegionEndpoint.GetBySystemName("us-east-2"),
                                    groupName,
                                    streamName
                                    ));
            var logger = LogFactory.GetLogger();
            logger.Warn("Message", ex, null, "Any String", new { FirstName = "Wes" });*/
            var ex = new Exception("Fake exception");
            cloudWatchLogger.Warn("Clean message");
            cloudWatchLogger.Warn("Cloudwatch Testing Stack", ex, null, "Any String", new { FirstName = "Wes" }, new { LastName = "Hendon" });

            Assert.True(true); // you must check cloudwatch to see if it worked
        }
    }
}
