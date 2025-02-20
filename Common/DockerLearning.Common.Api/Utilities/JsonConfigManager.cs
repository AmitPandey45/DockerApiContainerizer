namespace DockerLearning.Common.Api.Utilities
{
    using Newtonsoft.Json.Linq;
    using System;
    using System.IO;
    using System.Collections;
    using NLog;

    public static class JsonConfigManager
    {
        public static NLog.ILogger Logger;

        public static void UpdateJsonFile(string filePath, string key, string newValue)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return;
            }

            var jsonContent = File.ReadAllText(filePath);
            JObject jsonObj = JObject.Parse(jsonContent);
            var token = jsonObj.SelectToken(key);
            if (token != null)
            {
                token.Replace(newValue);
                Console.WriteLine($"Updated key: {key} with new value: {newValue}");
            }
            else
            {
                Console.WriteLine($"Key {key} not found in the file.");
            }

            File.WriteAllText(filePath, jsonObj.ToString());
        }

        public static void UpdateJsonFileFromEnvVars(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return;
            }

            var jsonContent = File.ReadAllText(filePath);
            JObject jsonObj = JObject.Parse(jsonContent);
            foreach (DictionaryEntry variable in Environment.GetEnvironmentVariables())
            {
                string envVarKey = variable.Key.ToString();
                string envVarValue = variable.Value.ToString();
                var token = jsonObj.SelectToken(envVarKey);
                if (token != null)
                {
                    token.Replace(envVarValue);
                    Console.WriteLine($"Updated key: {envVarKey} with value from environment variable: {envVarValue}");
                }
                else
                {
                    Console.WriteLine($"Key {envVarKey} not found in the JSON file.");
                }
            }

            File.WriteAllText(filePath, jsonObj.ToString());
        }

        public static void UpdateJsonFileWithFilteredEnvVars(string filePath, Dictionary<string, string> filteredEnvVars)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return;
            }

            // Read the existing content of the JSON file
            var jsonContent = File.ReadAllText(filePath);

            // Parse the JSON content into a JObject
            JObject jsonObj = JObject.Parse(jsonContent);

            // Loop through filtered environment variables and update the JSON file
            foreach (var envVar in filteredEnvVars)
            {
                string envVarKey = envVar.Key;
                string envVarValue = envVar.Value;

                // Handle key transformation (e.g., remove the APPSETTING_, CONNSTR_, or NLOGTARGET_ prefix)
                string jsonKey = TransformKeyForJson(envVarKey);

                // Try to match the key in the JSON and update the value if found
                var token = GetJsonToken(jsonObj, jsonKey);
                if (token != null)
                {
                    token.Replace(envVarValue); // Replace the value for the matched key
                    Console.WriteLine($"Updated key: {jsonKey} with value from environment variable: {envVarValue}");
                }
                else
                {
                    Console.WriteLine($"Key '{jsonKey}' not found in the JSON file.");
                }
            }

            // Save the updated JSON back to the file
            File.WriteAllText(filePath, jsonObj.ToString());
        }

        private static string TransformKeyForJson(string envVarKey)
        {
            if (envVarKey.StartsWith(ConfigConstants.AppSettingPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return envVarKey.Substring(ConfigConstants.AppSettingPrefix.Length);
            }

            if (envVarKey.StartsWith(ConfigConstants.ConnStrPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return envVarKey.Substring(ConfigConstants.ConnStrPrefix.Length);
            }

            if (envVarKey.StartsWith(ConfigConstants.NLogTargetPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return envVarKey.Substring(ConfigConstants.NLogTargetPrefix.Length);
            }

            return envVarKey;
        }

        private static JToken GetJsonToken(JObject jsonObj, string jsonKey)
        {
            if (string.IsNullOrEmpty(jsonKey))
            {
                throw new ArgumentNullException(nameof(jsonKey), "The key cannot be null or empty.");
            }

            var token = jsonObj[jsonKey];
            if (token == null)
            {
                token = jsonObj.Properties()
                    .FirstOrDefault(t => string.Equals(t.Name, jsonKey, StringComparison.OrdinalIgnoreCase))?.Value;
            }

            return token;
        }
    }
}
