namespace SlackAlertOwner.Notifier.Abstract
{
    using Model;
    using NodaTime;
    using System.Collections.Generic;

    public interface IShiftsCalendarService
    {
        IEnumerable<Shift> MonthCalendar(IEnumerable<PatronDay> patronDays, IEnumerable<TeamMate> teamMates);
    }
}