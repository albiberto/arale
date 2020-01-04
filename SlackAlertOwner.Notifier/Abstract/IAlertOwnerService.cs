namespace SlackAlertOwner.Notifier.Abstract
{
    using Model;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IAlertOwnerService
    {
        Task<(Shift today, Shift tomorrow)> GetShift(IEnumerable<TeamMate> teamMates);
        Task<IEnumerable<TeamMate>> GetTeamMates();
        Task<IEnumerable<PatronDay>> GetPatronDays();
        Task<IEnumerable<Shift>> GetCalendar(IEnumerable<TeamMate> teamMates);
        Task WriteCalendar(IEnumerable<IEnumerable<object>> values);
        Task ClearCalendar();
    }
}