namespace ConfigMerge.Logging
{
    public static class LoggerExtensions
    {
        public static void Trace(this ILogger logger, object message)
        {
            logger.Log(LogLevel.Trace, message);
        }

        public static void Debug(this ILogger logger, object message)
        {
            logger.Log(LogLevel.Debug, message);
        }

        public static void Log(this ILogger logger, object message)
        {
            logger.Log(LogLevel.Normal, message);
        }

        public static void Important(this ILogger logger, object message)
        {
            logger.Log(LogLevel.Important, message);
        }
    }
}