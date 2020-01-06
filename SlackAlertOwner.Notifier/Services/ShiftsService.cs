﻿namespace SlackAlertOwner.Notifier.Services
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
        readonly ITimeService _timeService;
        readonly IRandomIndexService _randomIndexService;
        
        public ShiftsService(Func<IEnumerable<LocalDate>> build , ITimeService timeService, IRandomIndexService randomIndexService)
        {
            _timeService = timeService;
            _randomIndexService = randomIndexService;

            _calendar = build();
            _patronDays = new List<PatronDay>();
        }

        public IEnumerable<Shift> Build(IEnumerable<TeamMate> teamMates)
        {
            var calendar = _calendar.Zip(Forever(teamMates), (day, entity) =>
                    new Shift(entity, day))
                .ToList();

            if (_patronDays.Any()) ApplyPatrons(calendar, _patronDays);
            
            return calendar;
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

        void ApplyPatrons(ICollection<Shift> shifts, ICollection<PatronDay> patronDays)
        {
            IEnumerable<Shift> GetShiftToBeSwitch() =>
                from patron in patronDays.Where(day => day.Day.Month == _timeService.Now.Month)
                from shift in shifts
                where shift.Schedule == patron.Day && shift.TeamMate.CountryCode == patron.CountryCode
                select shift;
            
            while (GetShiftToBeSwitch().Any())
            {
                var shiftToBeSwitch = GetShiftToBeSwitch().First();

                var candidates =
                    shifts.Where(shift => shift.TeamMate.CountryCode != shiftToBeSwitch.TeamMate.CountryCode).ToList();

                var randomTeamMateIndex = _randomIndexService.Random(candidates.Count());

                var candidateShift = candidates.Skip(randomTeamMateIndex).First();

                var temp = candidateShift.TeamMate.Clone() as TeamMate;

                var firstSwitch = shifts.First(s => s.Schedule == candidateShift.Schedule);
                firstSwitch.TeamMate = shiftToBeSwitch.TeamMate;

                var secondSwitch = shifts.First(s => s.Schedule == shiftToBeSwitch.Schedule);
                secondSwitch.TeamMate = temp;
            }
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

        static IEnumerable<T> Forever<T>(IEnumerable<T> source)
        {
            while (true)
                // ReSharper disable once PossibleMultipleEnumeration
                foreach (var item in source)
                    yield return item;
        }
    }
}