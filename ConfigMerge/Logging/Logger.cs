using System;

namespace ConfigMerge.Logging
{
    public static class Logger
    {
        public static LogLevel Level { get; set; }

        static Logger()
        {
            Level = LogLevel.Normal;
        }

        public static ILogger For<T>()
        {
            return For(typeof (T));
        }

        public static ILogger For(Type type)
        {
            return WithName(type.Name);
        }

        private static ILogger WithName(string name)
        {
            return new ConsoleLogger(name);
        }
    }

    public enum LogLevel
    {
        Trace,
        Debug,
        Normal,
        Important,
        Silent
    }
}