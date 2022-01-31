using System.IO;
using Microsoft.Extensions.Configuration;

namespace AspNetCore.CustomValidation.Demo
{
    public static class AppSettings
    {
        public static string GetValue(string key)
        {
            return GetConfiguration().GetSection(key).Value;
        }

        private static IConfigurationRoot GetConfiguration()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            IConfigurationRoot configuration = builder.Build();

            return configuration;
        }
    }
}
