using DockerLearning.Common.Api.Utilities;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

foreach (var variable in Environment.GetEnvironmentVariables())
{
    // Cast the variable to DictionaryEntry to access Key and Value
    var envVar = (System.Collections.DictionaryEntry)variable;
    Console.WriteLine($"{envVar.Key}: {envVar.Value}");
}

////var allEnvVars = Environment.GetEnvironmentVariables();
////foreach (var variable in Environment.GetEnvironmentVariables())
////{
////    // Cast the variable to DictionaryEntry to access Key and Value
////    var envVar = (System.Collections.DictionaryEntry)variable;
////    Console.WriteLine($"{envVar.Key}: {envVar.Value}");
////}

////builder.Configuration.AddEnvironmentVariables();

////// Access all environment variables
////foreach (var kvp in builder.Configuration.AsEnumerable())
////{
////    Console.WriteLine($"{kvp.Key} = {kvp.Value}");
////}

// Load configuration from base file and environment-specific file
////builder.Configuration
////    .SetBasePath(Directory.GetCurrentDirectory())
////    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) // Default file for Dev
////    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)  // For environment-specific overrides
////    .AddEnvironmentVariables(); // Load environment variables (if needed)

var logger = LogManager.GetLogger("Default");
AppConfigLoader.Logger = logger;
//JsonConfigManager.Logger = logger;

AppConfigLoader.SetAppConfig(builder);

//AppSettingsHelper.SetConfiguration(builder.Configuration);

//var environmentFile = $"appsettings.{builder.Environment.EnvironmentName}.json";
//Console.WriteLine($"Loading environment-specific file: {environmentFile}");

//var environment1 = builder.Configuration["ASPNETCORE_ENVIRONMENT"];
//var environment2 = AppSettingsHelper.GetValue("ASPNETCORE_ENVIRONMENT");
//var customEnv1 = AppSettingsHelper.GetValue("MY_CUSTOM_ENV1");
//var customEnv2 = AppSettingsHelper.GetValue("MY_CUSTOM_ENV2");
//Console.WriteLine($"ASPNETCORE_ENVIRONMENT: {environment1}");
//Console.WriteLine($"ASPNETCORE_ENVIRONMENT: {environment2}");
//Console.WriteLine($"MY_CUSTOM_ENV1: {customEnv1}");
//Console.WriteLine($"MY_CUSTOM_ENV2: {customEnv2}");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Now set up NLog
builder.Host.ConfigureLogging(logging =>
{
    // This clears any existing logging configurations (including Microsoft's)
    logging.ClearProviders();

    // Here, we directly use NLog's LogLevel to configure the minimum level for NLog
    // Set NLog's minimum level (e.g., Trace, Debug, Info)
    //logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Warning); // This still uses Microsoft's LogLevel for ASP.NET Core's internal use.

    logging.AddFilter("Microsoft", Microsoft.Extensions.Logging.LogLevel.None); // Or .Critical to allow only critical MS logs if needed
    logging.AddFilter("System", Microsoft.Extensions.Logging.LogLevel.None); // Or .Critical to allow only critical System logs if needed
    logging.AddFilter("Microsoft.AspNetCore", Microsoft.Extensions.Logging.LogLevel.None); // Or .Critical to allow only critical MS AspNetCore logs if needed

}).UseNLog(); // This adds NLog as the logging provider.


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

LogUtility.ConfigureNLog();

app.Run();
