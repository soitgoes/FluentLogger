using FluentLogger.Interfaces;

namespace FluentLogger
{
    public static class LogFactory
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static ILog logger;


        /// <summary>
        /// Init the specified loggers.
        /// Paramaters: Loogers Log
        /// </summary>
        public static void Init(params BaseLogger[] loggers)
        {
            logger = new CompositeLogger(loggers);
         }

        /// <summary>
        /// Inits the null logger.
        /// </summary>
        public static void InitNullLogger()
        {
            logger = new ConsoleLogger();
        }

        /// <summary>
        /// Gets the logger.
        /// Returns The Logger.
        /// </summary>
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
