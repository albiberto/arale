namespace SlackAlertOwner.Notifier.IoC
{
    using Abstract;
    using Adapters;
    using Clients;
    using Converters;
    using Factories;
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

    public static class RegisterServices
    {
        public static void AddOptionsPattern(this IServiceCollection services, IConfigurationBuilder builder)
        {
            var config = builder.Build();
            services.Configure<MyOptions>(config);
        }

        public static void AddQuartz(this IServiceCollection services)
        {
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            services.AddSingleton<NotifyJob>();
            services.AddSingleton(provider => new JobSchedule(typeof(NotifyJob),
                provider.GetService<IOptions<MyOptions>>().Value.NotifyJobCronExpression));

            services.AddSingleton<CalendarJob>();
            services.AddSingleton(provider => new JobSchedule(typeof(CalendarJob),
                provider.GetService<IOptions<MyOptions>>().Value.CalendarJobCronExpression));
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
            services.AddSingleton<ISheetsServiceFactory, SheetsServiceFactory>();
            services.AddSingleton<ITypeConverter<LocalDate>, LocalDateConverter>();
            services.AddSingleton<ITimeService, TimeService>();
            services.AddSingleton<IGoogleSpreadSheetClient, GoogleSpreadSheetClient>();
            services.AddSingleton<ICalendarService, CalendarService>();
            services.AddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));

            services.AddSingleton<IShiftsService>(provider => new ShiftsService(
                () => provider.GetService<ICalendarService>().WithoutHolidays().WithoutWeekEnd().Build()));
        }
    }
}