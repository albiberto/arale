namespace SlackAlertOwner.Notifier.Services
{
    using Abstract;
    using Model;
    using Nager.Date;
    using NodaTime;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ShiftsCalendarService : IShiftsCalendarService
    {
        readonly ITimeService _timeService;

        public ShiftsCalendarService(ITimeService timeService)
        {
            _timeService = timeService;
        }

        public IEnumerable<Shift> MonthCalendar(IEnumerable<PatronDay> patronDays, IEnumerable<TeamMate> teamMates)
        {
            var firstDay = _timeService.Now.With(DateAdjusters.StartOfMonth);
            
            var holidays = DateSystem.GetPublicHoliday(firstDay.Year, CountryCode.IT)
                .Select(publicHoliday => LocalDate.FromDateTime(publicHoliday.Date))
                .ToHashSet();

            var monthDaysNumber = DateTime.DaysInMonth(firstDay.Year, firstDay.Month);

            bool IsWorkDay(LocalDate day) =>
                !holidays.Contains(day) && day.DayOfWeek != IsoDayOfWeek.Sunday && day.DayOfWeek != IsoDayOfWeek.Saturday;

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