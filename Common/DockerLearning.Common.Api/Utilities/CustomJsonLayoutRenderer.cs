using NLog;
using NLog.Layouts;
using NLog.Config;
using System;
using System.Text;
using Newtonsoft.Json;
using NLog.LayoutRenderers;

namespace DockerLearning.Common.Api.Utilities
{
    [LayoutRenderer("customJsonLayout")]
    public class CustomJsonLayoutRenderer : LayoutRenderer
    {
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var logData = new Dictionary<string, object>();

            logData["level"] = logEvent.Level.Name;
            logData["message"] = logEvent.Message;
            logData["timestamp"] = logEvent.TimeStamp.ToString("o");
            logData["logger"] = logEvent.LoggerName;
            logData["exception"] = logEvent.Exception?.ToString() ?? string.Empty;

            foreach (var property in logEvent.Properties)
            {
                string key = property.Key?.ToString();

                if (key != null && property.Value != null)
                {
                    if (property.Value is string strValue)
                    {
                        logData[key] = strValue;
                    }
                    else if (property.Value is IDictionary<string, object> dictValue)
                    {
                        logData[key] = dictValue;
                    }
                    else if (property.Value is IEnumerable<KeyValuePair<string, object>> enumerableValue)
                    {
                        logData[key] = enumerableValue.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    }
                    else if (property.Value.GetType().IsPrimitive || property.Value is decimal)
                    {
                        logData[key] = property.Value;
                    }
                    else
                    {
                        logData[key] = property.Value.ToString(); // Fallback to ToString()
                    }
                }
            }

            var json = JsonConvert.SerializeObject(logData, Formatting.Indented);
            builder.Append(json);
        }
    }
}
