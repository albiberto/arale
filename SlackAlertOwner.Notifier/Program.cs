namespace SlackAlertOwner.Notifier
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using Model;
    using Quartz;
    using Quartz.Impl;
    using Quartz.Spi;
    using System;
    using System.IO;

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var configBuilder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", true);
                    var config = configBuilder.Build();

                    services.Configure<MyOptions>(config.GetSection("Options"));

                    services.AddSingleton<IJobFactory, SingletonJobFactory>();
                    services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
                    services.AddSingleton<NotifyJob>();

                    services.AddSingleton(provider => new JobSchedule(typeof(NotifyJob),
                        provider.GetService<IOptions<MyOptions>>().Value.CronExpression));

                    services.AddHostedService<QuartzHostedService>();

                    services.AddHttpClient("cazzeggingZoneClient",
                        (provider, client) =>
                        {
                            client.BaseAddress = new Uri(provider.GetService<IOptions<MyOptions>>().Value.BaseUrl);
                        });

                    services.AddSingleton<ISlackHttpClient, SlackHttpClient>();
                });
    }
}