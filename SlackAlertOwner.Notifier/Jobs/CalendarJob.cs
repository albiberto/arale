namespace SlackAlertOwner.Notifier.Jobs
{
    using Abstract;
    using Microsoft.Extensions.Logging;
    using Model;
    using Nager.Date;
    using NodaTime;
    using Quartz;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [DisallowConcurrentExecution]
    public class CalendarJob : IJob
    {
        readonly IAlertOwnerService _alertOwnerService;
        readonly ITypeConverter<LocalDate> _converter;
        readonly ISlackHttpClient _httpClient;
        readonly IShiftsService _shiftsService;
        readonly ILogger<NotifyJob> _logger;
        readonly ITimeService _timeService;

        public CalendarJob(IAlertOwnerService alertOwnerService, ISlackHttpClient httpClient,
           IShiftsService shiftsService,
            ITimeService timeService, ITypeConverter<LocalDate> converter,
            ILogger<NotifyJob> logger)
        {
            _alertOwnerService = alertOwnerService;
            _httpClient = httpClient;
            _shiftsService = shiftsService;
            _timeService = timeService;
            _converter = converter;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Start CalendarJob");

            await _alertOwnerService.ClearCalendar();

            var teamMates = await _alertOwnerService.GetTeamMates();
            var patronDays = await _alertOwnerService.GetPatronDays();

            var calendar = _shiftsService
                .AddPatronDays(patronDays)
                .Build(teamMates)
                .ToList();

            await _alertOwnerService.WriteCalendar(calendar.Select(day => new List<object>
            {
                _converter.FormatValueAsString(day.Schedule),
                $"{day.TeamMate.Name}"
            }));
            
            await _httpClient.Notify("Ciao <!channel> e' uscito il nuovo calendario dei turni:");
            await _httpClient.Notify(calendar.Select(shift =>
                $"{_converter.FormatValueAsString(shift.Schedule)} - {shift.TeamMate.Name}"));

            _logger.LogInformation("CalendarJob Completed");
        }
    }
}