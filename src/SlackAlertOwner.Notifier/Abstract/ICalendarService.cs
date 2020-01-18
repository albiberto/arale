namespace SlackAlertOwner.Notifier.Abstract
{
    using NodaTime;
    using Services;
    using System.Collections.Generic;

    public interface ICalendarService
    {
        IEnumerable<LocalDate> Build();
        CalendarService WithoutWeekEnd();
        CalendarService WithoutHolidays();
    }
}