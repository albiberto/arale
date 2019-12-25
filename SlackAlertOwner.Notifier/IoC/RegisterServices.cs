namespace SlackAlertOwner.Notifier.IoC
{
    using Abstract;
    using Clients;
    using Converters;
    using Jobs;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Model;
    using NodaTime;
    using Quartz;
    using Quartz.Impl;
    using Quartz.Spi;
    using Services;
    using System;
    using System.IO;

    public static class RegisterServices
    {
        public static void AddOptionsPattern(this IServiceCollection services)
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true);
            var config = configBuilder.Build();

            services.Configure<MyOptions>(config.GetSection("Options"));
        }

        public static void AddQuartz(this IServiceCollection services)
        {
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            services.AddSingleton<NotifyJob>();
            services.AddSingleton(provider => new JobSchedule(typeof(NotifyJob),
                provider.GetService<IOptions<MyOptions>>().Value.CronExpression));

            // services.AddSingleton<CalendarJob>();
            // services.AddSingleton(provider => new JobSchedule(typeof(CalendarJob),
            //     provider.GetService<IOptions<MyOptions>>().Value.CronExpression));
        }

        public static void AddHttpClients(this IServiceCollection services)
        {
            services.AddHttpClient("cazzeggingZoneClient",
                (provider, client) =>
                {
                    client.BaseAddress = new Uri(provider.GetService<IOptions<MyOptions>>().Value.BaseUrl);
                });
        }

        public static void AddEnvironment(this IServiceCollection services)
        {
            services.AddSingleton<ISlackHttpClient, SlackHttpClient>();
            services.AddSingleton<IAlertOwnerService, AlertOwnerService>();
            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<ITypeConverter<LocalDate>, LocalDateConverter>();
            services.AddSingleton<ITimeService, TimeService>();
            services.AddSingleton<IGoogleSpreadSheetService, SpreadSheetClient>();
        }
    }
}