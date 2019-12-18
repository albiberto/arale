﻿namespace SlackAlertOwner.Notifier.Abstract
{
    using Model;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ISpreadSheetService
    {
        Task<Shift> GetShift(IEnumerable<TeamMate> teamMates);
        Task<IEnumerable<TeamMate>> GetTeamMates();
        Task WriteCalendar(IEnumerable<TeamMate> teamMates);
        Task ClearCalendar();
        Task<IEnumerable<PatronDay>> GetPatronDays();
    }
}