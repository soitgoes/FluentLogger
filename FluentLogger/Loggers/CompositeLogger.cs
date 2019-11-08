using System;
using System.Collections.Generic;
using System.Linq;

namespace FluentLogger
{
    internal class CompositeLogger : BaseLogger
    {
        private List<BaseLogger> loggers = new List<BaseLogger>();

        public CompositeLogger(params BaseLogger[] loggers) : base(LogLevel.Trace)
        {
            this.loggers.AddRange(loggers);
        }

        public void AddLogger(BaseLogger logger)
        {
            this.loggers.Add(logger);
        }

        public void RemoveLogger<T>()
        {
            this.loggers = this.loggers.Where(x => !(x is T)).ToList();
        }

        public override void Record(LogLevel level, string message, Exception ex = null, params object[] objectsToSerialize)
        {
            foreach (var logger in loggers)
            {
                if (logger.MinLevel > level) continue;
                try
                {
                    logger.Record(level, message, ex, objectsToSerialize);
                }
                catch (Exception)
                {
                    //Nothing to do log failure
                }
            }
        }
    }
}
