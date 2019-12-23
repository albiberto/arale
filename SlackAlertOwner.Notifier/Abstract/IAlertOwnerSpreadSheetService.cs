﻿namespace SlackAlertOwner.Notifier.Abstract
{
    using Model;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IAlertOwnerSpreadSheetService
    {
        Task<Shift> GetShift(IEnumerable<TeamMate> teamMates);
        Task<IEnumerable<TeamMate>> GetTeamMates();
        Task WriteCalendar(IEnumerable<IEnumerable<object>> values);
        Task ClearCalendar();
        Task<IEnumerable<PatronDay>> GetPatronDays();
    }
}