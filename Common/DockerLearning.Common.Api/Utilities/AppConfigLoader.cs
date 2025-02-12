using NLog;
using System.Collections;

namespace DockerLearning.Common.Api.Utilities
{
    public static class AppConfigLoader
    {
        public static NLog.ILogger Logger;

        public static IConfiguration SetAppConfig(WebApplicationBuilder builder)
        {
            Console.WriteLine("Started SetAppConfig");
            var environmentName = builder.Environment.EnvironmentName;
            string environmentFile = $"appsettings.{environmentName}.json";
            environmentFile = environmentName.ToLower().Equals("dev") ? "appsettings.json" : environmentFile;
            Console.WriteLine($"GetCurrentDirectory: {Directory.GetCurrentDirectory()}");

            SetNLogConfig(environmentName, builder);
            Console.WriteLine($"SetNLogConfig Done");

            ////var allEnvVars = builder.Configuration.AsEnumerable();
            ////var filteredEnvVars = allEnvVars
            ////    .Where(envVar => envVar.Key.StartsWith("MY_CUSTOM_ENV"))
            ////    .ToDictionary(envVar => envVar.Key, envVar => envVar.Value);

            // Get all environment variables
            var allEnvVars = Environment.GetEnvironmentVariables();

            // Filter environment variables that start with "MY_CUSTOM_ENV"
            var filteredEnvVars = allEnvVars.Cast<DictionaryEntry>()
                .Where(envVar => envVar.Key.ToString().StartsWith("MY_CUSTOM_ENV"))
                .ToDictionary(envVar => envVar.Key.ToString(), envVar => envVar.Value.ToString());

            JsonConfigManager.UpdateJsonFileWithFilteredEnvVars(environmentFile, filteredEnvVars);

            builder.Configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(environmentFile, optional: true, reloadOnChange: false);

            Console.WriteLine($"AppSettings set: {environmentFile} Done");

            return builder.Configuration;
        }

        private static void SetNLogConfig(string environmentName, WebApplicationBuilder builder)
        {
            string nlogConfigFile = $"NLog.{environmentName}.config";
            nlogConfigFile = environmentName.ToLower().Equals("dev") ? "appsettings.json" : nlogConfigFile;
            Console.WriteLine($"Using NLog configuration file: {nlogConfigFile}");

            LogManager.Setup().LoadConfigurationFromFile(nlogConfigFile);

            ///dotnet add package NLog
            ///dotnet add package NLog.Web
            ///dotnet add package NLog.Extensions.Logging
            ///
            ////builder.Logging.ClearProviders();
            ////builder.Logging.AddNLog();
        }
    }
}
