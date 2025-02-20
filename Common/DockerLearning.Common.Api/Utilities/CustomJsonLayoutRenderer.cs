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
            var logData = new
            {
                level = logEvent.Level.Name,
                message = logEvent.Message,
                timestamp = logEvent.TimeStamp.ToString("o"),
                logger = logEvent.LoggerName,
                exception = logEvent.Exception?.ToString() ?? string.Empty,
            };

            // Serialize with indentation
            var json = JsonConvert.SerializeObject(logData, Formatting.Indented);
            builder.Append(json);
        }
    }
}
