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

            try
            {
                var objs = objectsToSerialize == null || objectsToSerialize.Any();
                message = $"[{level}] " + message + "\n" + ex?.Message + "\n" + (ex?.StackTrace ?? "") + (objs ? string.Join("\n", objectsToSerialize.Select(o => Serialize(o)).ToList()) : "");

                logEvents.Add(new InputLogEvent { Message = message, Timestamp = DateTime.Now });
            }
            catch (Exception _ex)
            {
                logEvents.Add(new InputLogEvent { Message = "Failed to build msg\n" + ex?.Message, Timestamp = DateTime.Now });
            }

            PutLogEventsResponse putResult = null;
            try
            {
                using (var logs = new AmazonCloudWatchLogsClient(accessKey, secret, region))
                {
                    var logStream = await GetLogStream();

                    if (logStream != null)
                        putResult = await PutLogs(logEvents, logStream.UploadSequenceToken);
                    else
                    {
                        await CreateLogStream();
                        logStream = await GetLogStream();
                        putResult = await PutLogs(logEvents, logStream?.UploadSequenceToken);
                    }
                }
            }
            catch (LimitExceededException)
            {
                // limit exceeded, not sure how to deal with this edge case
                Console.WriteLine($"(Limit Exceeded) Attempted to log message: " + message);
            }
            catch (InvalidSequenceTokenException _ex)
            {
                try
                {
                    // use the expected token
                    putResult = await PutLogs(logEvents, _ex.ExpectedSequenceToken);
                }
                catch (InvalidSequenceTokenException __ex)
                { // try one more time...
                    putResult = await PutLogs(logEvents, __ex.ExpectedSequenceToken);
                    Console.WriteLine($"(LoggingException) Exception: " + __ex.Message);
                }
            }
            catch (Exception _ex)
            {
                Console.WriteLine($"(LoggingException) Attempted to log message: " + message);
                Console.WriteLine($"(LoggingException) Exception: " + _ex.Message);
            }

            var myResult = putResult;
        }

        public async Task<LogStream> GetLogStream()
        {
            var streams = await DescribeLogStreams();
            return streams.FirstOrDefault(ls => ls.LogStreamName == logStreamName);
        }

        public async Task<List<LogStream>> DescribeLogStreams()
        {
            using (var logs = new AmazonCloudWatchLogsClient(accessKey, secret, region))
            {
                var describeLogStreamsRequest = new DescribeLogStreamsRequest(groupName);
                describeLogStreamsRequest.Descending = true;
                var describeLogStreamsResponse = await logs.DescribeLogStreamsAsync(describeLogStreamsRequest);
                //System.Net.HttpStatusCode.
                return describeLogStreamsResponse.LogStreams;
            }
        }

        public async Task<CreateLogStreamResponse> CreateLogStream()
        {
            try
            {
                using (var logs = new AmazonCloudWatchLogsClient(accessKey, secret, region))
                {
                    var createRequest = new CreateLogStreamRequest(groupName, logStreamName);
                    return await logs.CreateLogStreamAsync(createRequest);
                }
            }
            catch (ResourceAlreadyExistsException) { return null; }
        }

        public async Task<PutLogEventsResponse> PutLogs(List<InputLogEvent> logEvents, string token)
        {
            var request = new PutLogEventsRequest(groupName, logStreamName, logEvents);
            request.SequenceToken = token;

            using (var logs = new AmazonCloudWatchLogsClient(accessKey, secret, region))
            {
                return await logs.PutLogEventsAsync(request);
            }
        }
    }
}
