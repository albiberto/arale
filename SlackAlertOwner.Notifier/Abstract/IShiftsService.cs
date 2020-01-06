namespace SlackAlertOwner.Notifier.Abstract
{
    using Model;
    using Services;
    using System.Collections.Generic;

    public interface IShiftsService
    {
        public IEnumerable<Shift> Build(IEnumerable<TeamMate> teamMates);
        public IEnumerable<Shift> Build(IEnumerable<Shift> teamMates);
        public IShiftsService AddPatronDay(PatronDay patronDay);
        public IShiftsService AddPatronDays(IEnumerable<PatronDay> patronDays);
    }
}