namespace DockerLearning.Api
{
    public class WeatherForecast
    {
        public DateOnly Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }

        public string? AppKey1 { get; set; }

        public string? AppKey2 { get; set; }

        public string? CustomAppKey1 { get; set; }

        public string? CustomAppKey2 { get; set; }

        public string AwsKey { get; set; }

        public string AwsSecret { get; set; }
    }
}
