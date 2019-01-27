using FluentLogger.Interfaces;

namespace FluentLogger
{
    public static class LogFactory
    {
       
        private static CompositeLogger logger;

        public static void Init(params BaseLogger[] loggers)
        {
            logger = new CompositeLogger(loggers);
        }

        public static void AddLogger(BaseLogger loggerToAdd)
        {
            logger.AddLogger(loggerToAdd);
        }

        public static ILog InitNullLogger()
        {
            return new NullLogger();
        }
        public static ILog GetLogger()
        {
            if (logger == null)
            {
                var l = new ConsoleLogger();
                l.Info("Console Logger initiated.  Use init if other loggers are desired.");
                return l;
            }
            return logger;
        }
    }
}
