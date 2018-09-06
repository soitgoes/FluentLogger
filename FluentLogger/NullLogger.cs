using System;

namespace FluentLogger
{
    public class NullLogger: BaseLogger
    {
        public NullLogger(LogLevel minLevel = LogLevel.Trace) : base(minLevel)
        {
        }
        
        public override void Record(LogLevel level, string message, Exception ex = null, params object[] objectsToSerialize)
        {
            //Do nothing.  For Testing only
        }
        
    }
}
