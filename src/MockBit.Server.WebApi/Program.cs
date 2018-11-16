using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace MockBit.Server.WebApi
{
    public class Program
    {
        private static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args, Configuration).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args, IConfiguration configuration) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls($"http://*:{configuration["ListeningPorts:SetupApiPort"]}",
                         $"http://*:{configuration["ListeningPorts:MockApiPort"]}")
                .UseStartup<Startup>()
                .UseConfiguration(Configuration);
    }
}
