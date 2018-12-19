using System;

namespace FluentLogger
{
    public class ConsoleLogger : BaseLogger{
        public ConsoleLogger(LogLevel minLevel = LogLevel.Trace) : base(minLevel)
        {
        }


        public override void Record(LogLevel level, string message, Exception ex = null, params object[] objectsToSerialize)
        {
            SetColor(level);
            Console.WriteLine($"[{level.ToString().ToUpper()}] - {message}" + (ex == null ? "" : "-" + ex.StackTrace));
            foreach (var obj in objectsToSerialize)
            {
                //Console.WriteLine(obj.ToString());
                Console.WriteLine("-----------");
                Console.WriteLine(Serialize(obj));
            }
            Console.ResetColor();
        }

        private static void SetColor(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Fatal:
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    break;
                case LogLevel.Critical:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogLevel.Trace:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Warn:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
            }
        }
    }
}
