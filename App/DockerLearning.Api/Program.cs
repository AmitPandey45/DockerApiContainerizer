using DockerLearning.Common.Utilities;
using static System.Net.WebRequestMethods;

var builder = WebApplication.CreateBuilder(args);

// Load configuration from base file and environment-specific file
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) // Default file for Dev
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)  // For environment-specific overrides
    .AddEnvironmentVariables(); // Load environment variables (if needed)

AppSettingsHelper.SetConfiguration(builder.Configuration);

var environmentFile = $"appsettings.{builder.Environment.EnvironmentName}.json";
Console.WriteLine($"Loading environment-specific file: {environmentFile}");

var environment1 = builder.Configuration["ASPNETCORE_ENVIRONMENT"];
var environment2 = AppSettingsHelper.GetValue("ASPNETCORE_ENVIRONMENT");
var customEnv1 = AppSettingsHelper.GetValue("MY_CUSTOM_ENV1");
var customEnv2 = AppSettingsHelper.GetValue("MY_CUSTOM_ENV2");
Console.WriteLine($"Current Environment: {environment1}");
Console.WriteLine($"Current Environment: {environment2}");
Console.WriteLine($"Current Environment: {customEnv1}");
Console.WriteLine($"Current Environment: {customEnv2}");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.Run();
