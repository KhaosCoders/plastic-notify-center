using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace PlasticNotifyCenter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Windows Services start at the wrong directory
            bool isWinService = Directory.GetCurrentDirectory().Contains(@"\WINDOWS\system", StringComparison.CurrentCultureIgnoreCase);
            if (isWinService)
            {
                Directory.SetCurrentDirectory(AppContext.BaseDirectory);
            }

            // Setup logging
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.WithProperty("App Name", "PlasticNotifierCenter")
                .CreateLogger();
            
            Log.Information("Strating application");

            try
            {
                // Create ASP.NET webapp
                var hostBuilder = CreateHostBuilder(args);

                // Setup as Windows service, if started in system directory
                if (isWinService)
                {
                    hostBuilder.UseWindowsService();
                }

                // Run app
                hostBuilder.Build()
                           .Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                throw; // Throw exception, so Windows service crashes and exception will be logged to event prot
            }
            finally
            {
                Log.Information("Application stopped");
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog();

        /// <summary>
        /// Prepares appsettings.json and environment vars to be read
        /// </summary>
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }
}
