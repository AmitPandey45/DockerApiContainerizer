namespace DockerLearning.Common.Api.Logger
{
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using NLog;
    using System.Net;
    using System.Net.Mail;
    using System.Threading;

    public static class LogHelper
    {
        private static IHttpContextAccessor _httpContextAccessor;
        private static ILogger _logger;

        public static void Initialize(IHttpContextAccessor httpContextAccessor, ILogger logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public static void ConfigureNLog()
        {
            try
            {
                LogManager.Setup().LoadConfigurationFromFile("NLog.config");
            }
            catch (Exception ex)
            {
                LogHelper.Log(new LogOptions
                {
                    LogInfo = "ConfigureNLog",
                    LogLevel = LogLevel.Error,
                    Message = LogHelper.GetExceptionDetails(ex),
                    Context = null,
                    HttpStatusCode = 500
                });
            }
        }

        public static void Log(LogOptions options)
        {
            options.Context = _httpContextAccessor.HttpContext;
            string logMessage = GetLogMessage(options);
            if (_logger is NLog.Logger nlogLogger)
            {
                nlogLogger.Log(options.LogLevel, logMessage);
            }
            else
            {
                _logger.Log(options.LogLevel, logMessage);
            }
        }

        public static string GetLogMessage(LogOptions options)
        {
            string userId = null;
            string apiPath = null;
            string httpMethod = null;
            string userAgent = null;
            string ipAddress = null;
            int? httpStatusCode = null;

            if (options.Context != null)
            {
                userId = options.Context.Request.Headers["UserId"].FirstOrDefault();
                apiPath = options.Context.Request.Path;
                httpMethod = options.Context.Request.Method;
                options.UniqueRequestId = GetUniqueRequestId(options.Context);
                userAgent = options.Context.Request.Headers["User-Agent"].FirstOrDefault();
                ipAddress = GetIpAddress(options.Context);
                httpStatusCode = options.Context.Response.StatusCode;
            }

            var logEvent = new LogEventDetails
            {
                TimeStamp = DateTime.UtcNow.ToString("o"),
                LogLevel = options.LogLevel.ToString(),
                LogInfo = options.LogInfo,
                UniqueRequestId = options.UniqueRequestId,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                TenantId = "Test Tenant",
                AccountId = "Test Account",
                UserId = userId,
                IdentityDelegationUserId = userId,
                Api = apiPath,
                HttpMethod = httpMethod,
                HostName = Dns.GetHostName(),
                HttpStatusCode = httpStatusCode ?? options.HttpStatusCode,
                Message = options.Message,
                UserAgent = userAgent,
                IpAddress = ipAddress
            };

            return JsonConvert.SerializeObject(logEvent, Formatting.Indented);
        }

        public static string GetExceptionDetails(Exception ex) => $"Message => {ex.Message}, Stack Trace => {ex.ToString()}";

        public static string? GetUniqueRequestId(HttpContext? context) => context?.Request.Headers["UniqueRequestId"].ToString();

        public static string? GetIpAddress(HttpContext? context)
        {
            if (context == null) return null;

            var ip = context.Connection.RemoteIpAddress?.ToString();

            if (string.IsNullOrEmpty(ip) && context.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                ip = context.Request.Headers["X-Forwarded-For"];
            }

            return ip;
        }
    }
}
