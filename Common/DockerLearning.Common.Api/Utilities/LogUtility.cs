namespace DockerLearning.Common.Api.Utilities
{
    using NLog;
    using NLog.Config;
    using NLog.Layouts;
    using NLog.Targets;
    using System.Text.Json;

    public class LogUtility
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private static bool _isConfigured = false;

        public static void ConfigureNLog()
        {
            if (!_isConfigured)
            {
                try
                {
                    LogManager.Setup().LoadConfigurationFromFile("NLog.config");
                    _isConfigured = true;
                    //throw new Exception("Exception occurred");
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading NLog.config: {ex.Message}. Using programmatic config.");
                    // Programmatic Configuration (Fallback)
                    var config = new LoggingConfiguration();
                    var jsonLayout = new JsonLayout();
                    // ... (add JSON layout attributes as before) ...

                    // *** Add these essential JSON layout attributes ***
                    jsonLayout.Attributes.Add(new JsonAttribute("time", "${longdate}", true)); // Timestamp
                    jsonLayout.Attributes.Add(new JsonAttribute("level", "${level:upperCase=true}", true)); // Log level
                    jsonLayout.Attributes.Add(new JsonAttribute("logger", "${logger}", true)); // Logger name
                    jsonLayout.Attributes.Add(new JsonAttribute("message", "${message}", true)); // The log message
                    jsonLayout.Attributes.Add(new JsonAttribute("properties", "${all-event-properties}", true)); // All properties
                    jsonLayout.Attributes.Add(new JsonAttribute("exception", "${exception:format=toString}", true)); // Exception details

                    var fileTarget = new FileTarget("file")
                    {
                        FileName = @"C:\inetpub\wwwroot\logs\app-log.txt", // Change to your path
                        Layout = jsonLayout,
                        // ... (archive settings as before) ...
                    };

                    var consoleTarget = new ConsoleTarget("console")
                    {
                        Layout = jsonLayout
                    };

                    config.AddTarget("file", fileTarget);
                    config.AddTarget("console", consoleTarget);

                    config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, fileTarget));
                    config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, consoleTarget));

                    LogManager.Configuration = config;
                    _isConfigured = true;
                }
            }
        }

        public static void LogInfo(string message, object properties = null) => Log(LogLevel.Info, message, properties);
        public static void LogWarn(string message, object properties = null) => Log(LogLevel.Warn, message, properties);
        public static void LogError(string message, Exception ex = null, object properties = null) => Log(LogLevel.Error, message, properties, ex);
        public static void LogDebug(string message, object properties = null) => Log(LogLevel.Debug, message, properties);
        public static void LogFatal(string message, Exception ex = null, object properties = null) => Log(LogLevel.Fatal, message, properties, ex);

        private static void Log(LogLevel level, string message, object properties = null, Exception ex = null)
        {
            ConfigureNLog();

            var logEventInfo = new LogEventInfo(level, "MyLogger", message);

            // Only add properties if they are not null or empty
            if (properties != null)
            {
                if (properties is IDictionary<string, object> dict)
                {
                    foreach (var kvp in dict)
                    {
                        // Only add properties that are not null or empty
                        if (kvp.Value != null)
                        {
                            logEventInfo.Properties[kvp.Key] = kvp.Value;
                        }
                    }
                }
                else
                {
                    foreach (var property in properties.GetType().GetProperties())
                    {
                        var value = property.GetValue(properties);
                        // Only add properties that are not null or empty
                        if (value != null)
                        {
                            logEventInfo.Properties[property.Name] = value;
                        }
                    }
                }
            }

            if (ex != null)
            {
                logEventInfo.Exception = ex;
            }

            _logger.Log(logEventInfo);
        }

        //private static void Log(LogLevel level, string message, object properties = null, Exception ex = null)
        //{
        //    ConfigureNLog();

        //    var logEventInfo = new LogEventInfo(level, "MyLogger", message);

        //    if (properties != null)
        //    {
        //        if (properties is IDictionary<string, object> dict)
        //        {
        //            foreach (var kvp in dict)
        //            {
        //                logEventInfo.Properties[kvp.Key] = kvp.Value;
        //            }
        //        }
        //        else
        //        {
        //            foreach (var property in properties.GetType().GetProperties())
        //            {
        //                logEventInfo.Properties[property.Name] = property.GetValue(properties);
        //            }
        //        }
        //    }

        //    if (ex != null)
        //    {
        //        logEventInfo.Exception = ex;
        //    }

        //    _logger.Log(logEventInfo);
        //}

        public static string ToJson(object obj)
        {
            return JsonSerializer.Serialize(obj);
        }
    }
}
