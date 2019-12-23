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
        readonly IAlertOwnerSpreadSheetService _alertOwnerSpreadSheetService;
        readonly ILogger<NotifyJob> _logger;
        readonly ITimeService _timeService;
        readonly ITypeConverter<LocalDate> _converter;

        public CalendarJob(IAlertOwnerSpreadSheetService alertOwnerSpreadSheetService, ITimeService timeService, ITypeConverter<LocalDate> converter,
            ILogger<NotifyJob> logger)
        {
            _alertOwnerSpreadSheetService = alertOwnerSpreadSheetService;
            _timeService = timeService;
            _converter = converter;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Start CalendarJob");

            await _alertOwnerSpreadSheetService.ClearCalendar();

            var teamMates = await _alertOwnerSpreadSheetService.GetTeamMates();
            var patronDays = await _alertOwnerSpreadSheetService.GetPatronDays();

            var calendar = MonthCalendar(patronDays, teamMates)
                .Select(day => new List<object>
                {
                    $"{day.Schedule}",
                    $"{day.TeamMate.Name}"
                });

            await _alertOwnerSpreadSheetService.WriteCalendar(calendar);

            _logger.LogInformation("CalendarJob Completed");
        }

        IEnumerable<Shift> MonthCalendar(IEnumerable<PatronDay> patronDays, IEnumerable<TeamMate> teamMates)
        {
            var firstDay = _timeService.Now.With(DateAdjusters.StartOfMonth);

            var holidays = DateSystem.GetPublicHoliday(firstDay.Year, CountryCode.IT)
                .Select(publicHoliday => LocalDate.FromDateTime(publicHoliday.Date))
                .ToHashSet();

            var monthDaysNumber = DateTime.DaysInMonth(firstDay.Year, firstDay.Month);

            bool IsWorkDay(LocalDate day) =>
                !holidays.Contains(day) && day.DayOfWeek != IsoDayOfWeek.Sunday &&
                day.DayOfWeek != IsoDayOfWeek.Saturday;

            var days = Enumerable.Range(0, monthDaysNumber)
                .Select(firstDay.PlusDays)
                .Where(IsWorkDay);

            var shifts = days.Zip(Forever(teamMates), (day, mate) => new Shift(mate, day));

            // todo: check patron days.
            return shifts;
        }

        static IEnumerable<T> Forever<T>(IEnumerable<T> source)
        {
            while (true)
                // ReSharper disable once PossibleMultipleEnumeration
                foreach (var item in source)
                    yield return item;
        }
    }
}