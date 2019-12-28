namespace SlackAlertOwner.Notifier.Services
{
    using Abstract;
    using Nager.Date;
    using NodaTime;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CalendarService : ICalendarService
    {
        readonly ITimeService _timeService;
        readonly ICollection<Func<LocalDate, bool>> _conditions = new List<Func<LocalDate, bool>>();
        
        public CalendarService(ITimeService timeService)
        {
            _timeService = timeService;
        }

        public IEnumerable<LocalDate> Build()
        {
            var day = _timeService.Now.With(DateAdjusters.StartOfMonth);

            var monthDaysNumber = DateTime.DaysInMonth(day.Year, day.Month);

            var calendar = Enumerable.Range(0, monthDaysNumber).Select(day.PlusDays);

            calendar = _conditions.Aggregate(calendar, (current, condition) => current.Where(condition));

            return calendar;
        }

        public CalendarService WithoutWeekEnd()
        {
            _conditions.Add(day => day.DayOfWeek != IsoDayOfWeek.Sunday && day.DayOfWeek != IsoDayOfWeek.Saturday);
            return this;
        }

        public CalendarService WithoutHolidays()
        {
            var holidays = DateSystem.GetPublicHoliday(_timeService.Now.Year, CountryCode.IT)
                .Select(publicHoliday => LocalDate.FromDateTime(publicHoliday.Date))
                .ToHashSet();

            _conditions.Add(day => !holidays.Contains(day));

            return this;
        }
    }
}