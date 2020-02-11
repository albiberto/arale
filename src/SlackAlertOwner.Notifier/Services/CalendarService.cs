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
            var day = ChooseMonth();

            var monthDaysNumber = DateTime.DaysInMonth(day.Year, day.Month);

            var calendar = Enumerable.Range(0, monthDaysNumber).Select(day.PlusDays);

            calendar = _conditions.Aggregate(calendar, (current, condition) => current.Where(condition));

            return calendar;
        }

        public CalendarService WithoutWeekEnd()
        {
            _conditions.Add(day => 
                day.DayOfWeek != IsoDayOfWeek.Sunday && day.DayOfWeek != IsoDayOfWeek.Saturday);
            
            return this;
        }

        public CalendarService WithoutHolidays()
        {
            var holidays = DateSystem.GetPublicHoliday(_timeService.Now.Year, CountryCode.IT)
                .Select(publicHoliday => LocalDate.FromDateTime(publicHoliday.Date));

            _conditions.Add(day => 
                !holidays.Contains(day));

            return this;
        }

        public LocalDate ChooseMonth()
        {

            LocalDate lastWorkingDayOfMonth = _timeService.Now.With(DateAdjusters.EndOfMonth);

            if (_timeService.Now.Equals(lastWorkingDayOfMonth))
            {
                return _timeService.NextMonth.With(DateAdjusters.StartOfMonth);
            }

            while (lastWorkingDayOfMonth.DayOfWeek.Equals(NodaTime.IsoDayOfWeek.Saturday) ||
                    lastWorkingDayOfMonth.DayOfWeek.Equals(NodaTime.IsoDayOfWeek.Sunday))
            {
                lastWorkingDayOfMonth = lastWorkingDayOfMonth.Minus(Period.FromDays(1));
            }

            if (_timeService.Now.Equals(lastWorkingDayOfMonth))
            {
                return _timeService.NextMonth.With(DateAdjusters.StartOfMonth);
            }

            return _timeService.Now.With(DateAdjusters.StartOfMonth);
        }
    }
}