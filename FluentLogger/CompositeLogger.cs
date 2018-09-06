using System;

namespace FluentLogger
{
    public class CompositeLogger : BaseLogger
    {
        private readonly BaseLogger[] loggers;

        public CompositeLogger(params BaseLogger[] loggers) : base(LogLevel.Trace)
        {
            this.loggers = loggers;
        }



        public override void Record(LogLevel level, string message, Exception ex = null, params object[] objectsToSerialize)
        {
            foreach (var logger in loggers)
            {
                logger.Record(level, message, ex, objectsToSerialize);
            }
        }
    }
}
