using NUnit.Framework;
using FluentLogger;
using Amazon;
using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;

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
            groupName = "GroupTest";
            streamName = "test-stream";

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
    }
}
