namespace DockerLearning.Common.Api.Logger
{
    public static class LoggerExtensions
    {
        public static void LogTrace(this NLog.ILogger logger, LogOptions options) => LogHelper.Log(options.WithLogLevel(NLog.LogLevel.Trace));

        public static void LogDebug(this NLog.ILogger logger, LogOptions options) => LogHelper.Log(options.WithLogLevel(NLog.LogLevel.Debug));

        public static void LogInfo(this NLog.ILogger logger, LogOptions options) => LogHelper.Log(options.WithLogLevel(NLog.LogLevel.Info));

        public static void LogWarn(this NLog.ILogger logger, LogOptions options) => LogHelper.Log(options.WithLogLevel(NLog.LogLevel.Warn));

        public static void LogError(this NLog.ILogger logger, LogOptions options) => LogHelper.Log(options.WithLogLevel(NLog.LogLevel.Error));

        public static void LogFatal(this NLog.ILogger logger, LogOptions options) => LogHelper.Log(options.WithLogLevel(NLog.LogLevel.Fatal));
    }
}
