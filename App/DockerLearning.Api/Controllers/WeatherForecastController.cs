using DockerLearning.Common.Utilities;
using Microsoft.AspNetCore.Mvc;

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

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IConfiguration config;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration config)
        {
            _logger = logger;
            this.config = config;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
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
