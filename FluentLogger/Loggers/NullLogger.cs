using System;

namespace FluentLogger
{
    public class NullLogger : BaseLogger
    {
        public NullLogger(LogLevel minLevel = LogLevel.Trace) : base(minLevel)
        {
        }
        /// <summary>
        /// For Testing Only.  Does Nothing
        /// </summary>
        /// <param name="level">Error Level</param>
        /// <param name="message">Message Details</param>
        /// <param name="ex">Exception</param>
        /// <param name="objectsToSerialize">Object_Of_Message</param>
        public override void Record(LogLevel level, string message, Exception ex = null, params object[] objectsToSerialize)
        {
            //Do nothing.  For Testing only
        }

    }
}
