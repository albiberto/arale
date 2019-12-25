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
        readonly ISlackHttpClient _httpClient;
        readonly ILogger<NotifyJob> _logger;
        readonly ITimeService _timeService;
        readonly ITypeConverter<LocalDate> _converter;

        public CalendarJob(IAlertOwnerService alertOwnerService, ISlackHttpClient httpClient, ITimeService timeService, ITypeConverter<LocalDate> converter,
            ILogger<NotifyJob> logger)
        {
            _alertOwnerService = alertOwnerService;
            _httpClient = httpClient;
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

            var calendar = MonthCalendar(patronDays, teamMates).ToList();

            await _alertOwnerService.WriteCalendar(calendar.Select(day => new List<object>
            {
                _converter.FormatValueAsString(day.Schedule),
                $"{day.TeamMate.Name}"
            }));

            await _httpClient.Notify("Ciao <!channel> e' uscito il nuovo calendario dei turni:");
            await _httpClient.Notify(calendar.Select(shift => $"{shift.Schedule} - {shift.TeamMate.Id}"));

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

            var shifts = days.Zip(Forever(teamMates), (day, mate) => new Shift(mate, day)).ToList();

            var monthCalendar = shifts.ToList();
            var patrons = patronDays.ToList();
            
            Switch(monthCalendar, patrons);
            
            return monthCalendar;
        }

        void Switch(IReadOnlyCollection<Shift> shifts, IReadOnlyCollection<PatronDay> patronDays)
        {
            IEnumerable<Shift> GetShiftToBeSwitch() =>
                from patron in patronDays.Where(day => day.Day.Month == _timeService.Now.Month)
                from shift in shifts
                where shift.Schedule == patron.Day && shift.TeamMate.CountryCode == patron.CountryCode
                select shift;

            while (GetShiftToBeSwitch().Any())
            {
                var shiftToBeSwitch = GetShiftToBeSwitch().First();

                var candidates=
                    shifts.Where(shift => shift.TeamMate.CountryCode != shiftToBeSwitch.TeamMate.CountryCode).ToList();
                
                var randomTeamMateIndex = new Random().Next(0, candidates.Count() - 1);
                
                var candidateShift = candidates.Skip(randomTeamMateIndex).Take(1).First();

                var temp = candidateShift.TeamMate.Clone() as TeamMate;
                
                var firstSwitch = shifts.First(s => s.Schedule == candidateShift.Schedule);
                firstSwitch.TeamMate = shiftToBeSwitch.TeamMate;

                var secondSwitch = shifts.First(s => s.Schedule == shiftToBeSwitch.Schedule);
                secondSwitch.TeamMate = temp;
            };
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