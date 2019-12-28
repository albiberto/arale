namespace SlackAlertOwner.Notifier.Abstract
{
    using Model;
    using Services;
    using System.Collections.Generic;

    public interface IShiftsService
    {
        public IEnumerable<Shift> Build(IEnumerable<TeamMate> teamMates);
        public ShiftsService AddPatronDay(PatronDay patronDay);
        public ShiftsService AddPatronDays(IEnumerable<PatronDay> patronDays);
    }
}