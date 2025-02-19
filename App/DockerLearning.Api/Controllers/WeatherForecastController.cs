using DockerLearning.Common.Api.Utilities;
using DockerLearning.Common.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DockerLearning.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly NLog.ILogger _logger;
        private readonly IConfiguration config;

        public WeatherForecastController(IConfiguration config)
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
            this.config = config;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            NLog.ILogger logger = NLog.LogManager.GetCurrentClassLogger();
            var logDetails = new
            {
                Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                LogLevel = "TRACE",
                Message = "GetAlert => Testing for logging",
                UserId = "UnknownUser",
                ApiPath = "v1/api/Alert/",
                HttpMethod = "GET",
                HostName = Dns.GetHostName(),
                HttpStatusCode = 200,
                UniqueRequestId = Guid.NewGuid().ToString(),
            };
            logger.Trace(logDetails);

            LogUtility.LogInfo("MyMethod started.");
            LogUtility.LogDebug("Detailed debug info", new { Step = 1, Value = 10 });
            LogUtility.LogError("An error occurred", new Exception ("Error occurred for reading data"), new { ErrorCode = 500 });

            var appKey1 = config["AppConfig1"];
            var appKey2 = config["AppConfig2"];
            var customKey1 = config["MY_CUSTOM_ENV1"];
            var customKey2 = config["MY_CUSTOM_ENV2"];

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
                AppKey1 = appKey1,
                AppKey2 = appKey2,
                CustomAppKey1 = customKey1,
                CustomAppKey2 = customKey2,
            })
            .ToArray();
        }
    }
}
