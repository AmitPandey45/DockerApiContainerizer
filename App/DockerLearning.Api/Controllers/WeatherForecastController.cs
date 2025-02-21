using DockerLearning.Common.Api.Logger;
using DockerLearning.Common.Api.Utilities;
using DockerLearning.Common.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using NLog.Fluent;
using System.Net;
using System.Net.Mail;

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

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly NLog.ILogger _logger;
        private readonly IConfiguration config;

        public WeatherForecastController(IHttpContextAccessor httpContextAccessor, IConfiguration config, NLog.ILogger logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            this.config = config;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            LogHelper.Log(new LogOptions
            {
                LogLevel = NLog.LogLevel.Trace,
                LogInfo = "GetWeatherForecast",
                UniqueRequestId = Guid.NewGuid().ToString(),
                Message = "Started this GetWeatherForecast",
            });

            _logger.Trace("MyMethod started.");
            _logger.Debug("Detailed debug info");
            _logger.Warn("There is warning for one issue");
            _logger.Info("This is an info message");
            _logger.Fatal("This is Fatal message");
            _logger.Error("An error occurred", new Exception("Error occurred for reading data"), new { ErrorCode = 500 });

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
                AwsKey = config["AwsAccessKey"],
                AwsSecret = config["AwsSecretAccessKey"],
            })
            .ToArray();
        }
    }
}
