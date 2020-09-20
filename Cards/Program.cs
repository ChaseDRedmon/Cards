using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cards.Data.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace Cards
{
    public class Program
    {
        public static int Main(string[] args)
        {
            // Add the connections.json file to the configuration builder
            var config = new ConfigurationBuilder()
                .AddJsonFile("connections.json", optional: false, reloadOnChange: true)
                .Build();

            // Build our Serilog configuration
            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.WithThreadId()
                .WriteTo.Async(x => x.Console(restrictedToMinimumLevel: LogEventLevel.Debug,
                    outputTemplate:
                    "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} <{ThreadId}>{NewLine}{Exception}"))
                .WriteTo.Async(x => x.RollingFile(@"logs\{Date}", restrictedToMinimumLevel: LogEventLevel.Debug,
                    outputTemplate:
                    "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} <{ThreadId}>{NewLine}{Exception}"));

            // Find our SentryIO token from the connections.json file
            var sentryIoToken = config.GetValue<string>(nameof(CardsConfig.SentryIOToken));

            // Pass that value to our Sentry sink
            if (!string.IsNullOrWhiteSpace(sentryIoToken))
            {
                loggerConfig.WriteTo.Async(
                    x => x.Sentry(sentryIoToken, restrictedToMinimumLevel: LogEventLevel.Warning));
            }

            // Build our webhost
            var webHost = CreateWebHostBuilder(args, config).Build();

            // Create our logger
            Log.Logger = loggerConfig.CreateLogger();

            // Run the webhost
            try
            {
                webHost.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.ForContext<Program>()
                    .Fatal(ex, "Host terminated unexpectedly.");

                if (Debugger.IsAttached && Environment.UserInteractive)
                {
                    Console.WriteLine(Environment.NewLine + "Press any key to exit...");
                    Console.ReadKey(true);
                }

                return ex.HResult;
            }
            finally
            {
                // Close serilog and flush
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        /// Create our webhost configuration and point it towards out startup class where services are configured.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="config">Configuration</param>
        /// <returns>IWebHostBuilder instance</returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args, IConfiguration config) =>
            WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(config)
                .UseSerilog()
                .UseStartup<Startup>();
    
    }
}