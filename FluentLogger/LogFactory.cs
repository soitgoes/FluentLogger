using FluentLogger.Interfaces;

namespace FluentLogger
{
    public static class LogFactory
    {
       
        private static ILog logger;

        public static void Init(params BaseLogger[] loggers)
        {
            logger = new CompositeLogger(loggers);
         }

        public static void InitNullLogger()
        {
            logger = new ConsoleLogger();
        }
        public static ILog GetLogger()
        {
            if (logger == null)
            {
                logger = new ConsoleLogger();
                logger.Info("Console Logger initiated.  Use init if other loggers are desired.");
            }
                
            return logger;
        }
    }
}
