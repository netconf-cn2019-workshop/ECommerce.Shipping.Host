using ECommerce.Services.Common.Configuration;
using ECommerce.Shipping.Host.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ECommerce.Common.Infrastructure.Messaging;
using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace ECommerce.Shipping.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
               .MinimumLevel.Override("System", LogEventLevel.Warning)
               .Enrich.FromLogContext()
               .WriteTo.Console()
               .CreateLogger();

            try
            {
                Log.Information("服务正在启动");
                BuildHost(args).Run();
                return;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "服务意外终止");
                return;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHost BuildHost(string[] args) =>
            new HostBuilder()
                .ConfigureHostConfiguration(builder =>
                {
                    builder.SetBasePath(Directory.GetCurrentDirectory());
                    builder.AddEnvironmentVariables(prefix: "ASPNETCORE_");
                })
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.SetBasePath(Directory.GetCurrentDirectory());
                    builder.AddJsonFile($"appsettings.json", optional: false);
                    builder.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: false);
                    builder.AddEnvironmentVariables();
                    builder.AddCloud();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IMessageCorrelationContextAccessor, MessageCorrelationContextAccessor>();
                    services.AddScoped<IHostedService, ShippingService>();
                })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                })
                .UseSerilog()
                .UseServiceProviderFactory(new DependencyProvider())
                .UseConsoleLifetime()
                .Build();
    }
}
