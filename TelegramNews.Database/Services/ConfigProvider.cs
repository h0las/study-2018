namespace TelegramNews.Database.Services
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using System.IO;

    public static class ConfigProvider
    {
        private static IConfiguration Configuration;

        public static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder().SetBasePath("C:\\study-2018\\TelegramNews.Database").AddJsonFile("config.json");
            builder.AddJsonFile("appsettings.json", optional: true);
            Configuration = builder.Build();

            return Configuration;
        }
    }
}