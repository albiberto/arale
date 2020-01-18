namespace SlackAlertOwner.Notifier.Services
{
    using Abstract;
    using Model;
    using NodaTime;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ShiftsService : IShiftsService
    {
        readonly IEnumerable<LocalDate> _calendar;
        readonly ICollection<PatronDay> _patronDays;

        public ShiftsService(Func<IEnumerable<LocalDate>> build)
        {
            _calendar = build();
            _patronDays = new List<PatronDay>();
        }

        public IEnumerable<Shift> Build(IEnumerable<TeamMate> teamMates)
        {
            var leastRecentlyUsed = teamMates.ToList();

            TeamMate GetTeamMate(Predicate<TeamMate> predicate)
            {
                var index = leastRecentlyUsed.FindIndex(predicate);
                if (index == -1) return null;

                var teamMate = leastRecentlyUsed[index];
                leastRecentlyUsed.RemoveAt(index);
                leastRecentlyUsed.Add(teamMate);

                return teamMate;
            }

            bool CanWork(TeamMate mate, LocalDate date) => !_patronDays.Any(patronDay =>
                patronDay.Day.Month == date.Month && patronDay.Day.Day == date.Day &&
                patronDay.CountryCode == mate.CountryCode);

            foreach (var day in _calendar)
            {
                var teamMate = GetTeamMate(mate => CanWork(mate, day));
                yield return new Shift(teamMate, day);
            }
        }

        public IEnumerable<Shift> Build(IEnumerable<Shift> shifts)
        {
            var teamMates = ExtractTeamMatesFromOldCalendar(shifts);
            return Build(teamMates);
        }

        public IShiftsService AddPatronDay(PatronDay patronDay)
        {
            _patronDays.Add(patronDay);
            return this;
        }

        public IShiftsService AddPatronDays(IEnumerable<PatronDay> patronDays)
        {
            foreach (var patronDay in patronDays) _patronDays.Add(patronDay);
            return this;
        }

        static IEnumerable<TeamMate> ExtractTeamMatesFromOldCalendar(IEnumerable<Shift> shifts)
        {
            var teamMates = shifts
                .Reverse()
                .Select(shift => shift.TeamMate)
                .ToList();

            var teamMate = teamMates.First();

            var reordered = teamMates
                .Skip(1)
                .TakeWhile(mate => !string.Equals(mate.Id, teamMate.Id, StringComparison.InvariantCulture))
                .Reverse()
                .ToList();

            reordered.Add(teamMate);

            return reordered;
        }
    }
}