namespace FluentLogger.Console
{
    using Console = System.Console;
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("You must provide a file");
                return;
            }
            var watcher = new LogWatcher(args);
            watcher.Run().Wait();
        }

    }
}
