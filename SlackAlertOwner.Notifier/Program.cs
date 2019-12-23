namespace SlackAlertOwner.Notifier
{
    using Abstract;
    using Clients;
    using Converters;
    using IoC;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using NodaTime;
    using Services;

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
                    services.AddOptionsPattern();
                    services.AddQuartz();
                    services.AddHttpClients();

                    services.AddSingleton<ISlackHttpClient, SlackHttpClient>();
                    services.AddSingleton<IAlertOwnerSpreadSheetService, AlertOwnerSpreadSpreadSheetService>();
                    services.AddSingleton<IGoogleAuthenticationService, GoogleAuthenticationService>();
                    services.AddSingleton<ITypeConverter<LocalDate>, LocalDateConverter>();
                    services.AddSingleton<IShiftsCalendarService, ShiftsCalendarService>();
                    services.AddSingleton<ITimeService, TimeService>();
                    services.AddSingleton<ISpreadSheetService, SpreadSheetService>();

                    services.AddHostedService<QuartzHostedService>();
                });
    }
}