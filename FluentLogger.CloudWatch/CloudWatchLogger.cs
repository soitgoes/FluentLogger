using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using FluentLogger.Interfaces;

namespace FluentLogger.CloudWatch
{
    public class CloudWatchLogger : BaseLogger
    {
        private readonly string accessKey;
        private readonly string secret;
        private readonly RegionEndpoint region;
        private readonly string groupName;
        private readonly string logStreamName;

        public CloudWatchLogger(LogLevel minLevel, string accessKey, string secret, RegionEndpoint region, string groupName, string logStreamName) : base(minLevel)
        {
            this.accessKey = accessKey;
            this.secret = secret;
            this.region = region;
            this.groupName = groupName;
            this.logStreamName = logStreamName;
        }

        public override void Record(LogLevel level, string message, Exception ex = null, params object[] objectsToSerialize)
        {
            _ = RecordAsync(level, message, ex, objectsToSerialize);
        }

        public async Task RecordAsync(LogLevel level, string message, Exception ex = null, params object[] objectsToSerialize)
        {
            
            var logEvents = new List<InputLogEvent>();

            message = $"[{level}] " + message + "\n" + (ex?.StackTrace ?? "") +  objectsToSerialize.Select(o => Serialize(o) + "\n");

            logEvents.Add(new InputLogEvent { Message = message, Timestamp = DateTime.Now });

            using (var logs = new AmazonCloudWatchLogsClient(accessKey, secret, region))
            {
                var request = new PutLogEventsRequest(groupName, logStreamName, logEvents);

                var describeLogStreamsRequest = new DescribeLogStreamsRequest(groupName);
                var describeLogStreamsResponse = await logs.DescribeLogStreamsAsync(describeLogStreamsRequest);
                var logStreams = describeLogStreamsResponse.LogStreams;
                var logStream = logStreams.FirstOrDefault(ls => ls.LogStreamName == logStreamName);
                if (logStream != null)
                {
                    var token = logStream.UploadSequenceToken;
                    request.SequenceToken = token;
                    await logs.PutLogEventsAsync(request);
                }
                else
                {
                    var createRequest = new CreateLogStreamRequest(groupName, logStreamName);
                    await logs.CreateLogStreamAsync(createRequest);
                    await logs.PutLogEventsAsync(request);
                }
            }
        }
    }
}
