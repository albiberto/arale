namespace SlackAlertOwner.Notifier
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Quartz;
    using Quartz.Impl;
    using Quartz.Spi;
    using System;

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
                    services.AddSingleton<IJobFactory, SingletonJobFactory>();
                    services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
                    services.AddSingleton<NotifyJob>();

                    services.AddSingleton(new JobSchedule(
                        typeof(NotifyJob),
                        "0/20 * * * * ?")); // run every 5 seconds

                    services.AddHostedService<QuartzHostedService>();

                    services.AddHttpClient("cazzeggingZoneClient",
                        client =>
                        {
                            client.BaseAddress =
                                new Uri(
                                    "https://hooks.slack.com/services/T1N39MNKG/BRC7YPNSK/");
                        });
                    
                    services.AddSingleton<ISlackHttpClient, SlackHttpClient>();
                });
    }
}