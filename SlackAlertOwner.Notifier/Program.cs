namespace SlackAlertOwner.Notifier
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Quartz;
    using Quartz.Impl;
    using Quartz.Spi;

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

                    // Add our job
                    services.AddSingleton<NotifyJob>();
                    services.AddSingleton(new JobSchedule(
                        jobType: typeof(NotifyJob),
                        cronExpression: "0/5 * * * * ?")); // run every 5 seconds
                    
                    services.AddHostedService<QuartzHostedService>();
                    
                });
    }
}