using System;
using System.Collections.Generic;

namespace ConfigMerge.Logging
{
    public class ConsoleLogger : ILogger
    {
        private readonly string _name;

        public ConsoleLogger(string name)
        {
            _name = name;
        }

        public void Log(LogLevel level, object message)
        {
            if (level < Logger.Level)
            {
                return;
            }
            using (ConsoleColorer.For(level))
            {
                Console.WriteLine($"{_name}: {message}");
            }
        }

        private class ConsoleColorer : IDisposable
        {
            private static readonly IDictionary<LogLevel, ConsoleColor> Colors = new Dictionary<LogLevel, ConsoleColor>
            {
                {LogLevel.Trace, ConsoleColor.Gray },
                {LogLevel.Debug, ConsoleColor.Yellow },
                {LogLevel.Normal, ConsoleColor.White },
                {LogLevel.Important, ConsoleColor.Cyan },
                {LogLevel.Silent, ConsoleColor.Red }
            };

            private ConsoleColorer(LogLevel level)
            {
                Console.ForegroundColor = Colors[level];
            }

            public static ConsoleColorer For(LogLevel level)
            {
                return new ConsoleColorer(level);
            }

            public void Dispose()
            {
                Console.ResetColor();
            }
        }
    }
}