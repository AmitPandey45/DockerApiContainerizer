using Microsoft.Extensions.Configuration;

namespace DockerLearning.Common.Utilities
{
    public static class AppSettingsHelper
    {
        private static IConfiguration? _configuration;

        static AppSettingsHelper()
        {
        }

        public static void SetConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Get configuration value as a string
        public static string GetValue(string key)
        {
            if (_configuration == null)
                throw new InvalidOperationException("Configuration is not set.");

            return _configuration[key];
        }

        public static string GetValue(string key, string defaultValue)
        {
            if (_configuration == null)
                throw new InvalidOperationException("Configuration is not set.");

            return _configuration[key] ?? defaultValue;
        }

        public static T GetSection<T>(string sectionName) where T : class, new()
        {
            if (_configuration == null)
                throw new InvalidOperationException("Configuration is not set.");

            var section = _configuration.GetSection(sectionName);
            if (section.Exists())
            {
                return section.Get<T>();
            }
            return new T();
        }
    }
}
