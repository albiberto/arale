namespace SlackAlertOwner.Notifier.Abstract
{
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IAlertOwnerService
    {
        Task<(Shift today, Shift tomorrow)> GetShift(IEnumerable<TeamMate> teamMates);
        Task<IEnumerable<TeamMate>> GetTeamMates();
        Task WriteCalendar(IEnumerable<IEnumerable<object>> values);
        Task ClearCalendar();
        Task<IEnumerable<PatronDay>> GetPatronDays();
    }
}