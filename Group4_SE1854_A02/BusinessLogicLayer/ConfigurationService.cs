using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace BusinessLogicLayer
{
    // Service to handle application configuration (e.g., Admin Account)
    public class ConfigurationService
    {
        private IConfiguration _configuration;

        public ConfigurationService()
        {
            // Build configuration from appsettings.json
            _configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
        }

        public (string Email, string Password) GetAdminAccount()
        {
            string email = _configuration["AdminAccount:Email"];
            string password = _configuration["AdminAccount:Password"];
            return (email, password);
        }
    }
}
