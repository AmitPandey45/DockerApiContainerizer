namespace DockerLearning.Common.Api.Logger
{
    public class LogOptions
    {
        public string LogInfo { get; set; }

        public NLog.LogLevel LogLevel { get; set; }

        public string Message { get; set; }

        public HttpContext? Context { get; set; }

        public int? HttpStatusCode { get; set; }

        public string? UniqueRequestId { get; set; }

        public LogOptions WithLogLevel(NLog.LogLevel logLevel)
        {
            LogLevel = logLevel;
            return this;
        }
    }
}
