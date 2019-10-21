using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace AspNetCore.CustomValidation.Demo
{
    public class AppSettings
    {
        public static string GetValue(string key)
        {
           return GetConfiguration().GetSection(key).Value;
        }
        
        private static IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            IConfigurationRoot configuration = builder.Build();

            return configuration;
        }
    }
}
