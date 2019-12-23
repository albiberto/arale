namespace SlackAlertOwner.Notifier.Services
{
    using Abstract;
    using Microsoft.Extensions.Options;
    using Model;
    using NodaTime;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class AlertOwnerSpreadSpreadSheetService : IAlertOwnerSpreadSheetService
    {
        readonly ITypeConverter<LocalDate> _converter;
        readonly MyOptions _options;
        readonly ISpreadSheetService _spreadSheetService;

        public AlertOwnerSpreadSpreadSheetService(ISpreadSheetService spreadSheetService, IOptions<MyOptions> options,
            IShiftsCalendarService shiftsCalendarService, ITypeConverter<LocalDate> converter)
        {
            _spreadSheetService = spreadSheetService;
            _converter = converter;
            _options = options.Value;
        }

        public async Task<Shift> GetShift(IEnumerable<TeamMate> teamMates)
        {
            // TODO: Move SpreadSheetId in constructor.
            var shiftsCalendar = await _spreadSheetService.Get(_options.SpreadsheetId, _options.CalendarRange);

            var now = LocalDate.FromDateTime(DateTime.Now);

            return (from shift in shiftsCalendar.Values
                    let schedule = _converter.ParseValueFromString(shift.ElementAt(1) as string)
                    let teamMate = $"{shift.ElementAt(0)}"
                    where schedule == now
                    select new Shift(new TeamMate
                    {
                        Id = teamMates.First(tm => tm.Name.Contains(teamMate)).Id,
                        Name = teamMate
                    }, schedule)
                ).First();
        }

        public async Task<IEnumerable<PatronDay>> GetPatronDays()
        {
            var patronDays = await _spreadSheetService.Get(_options.SpreadsheetId, _options.PatronDaysRange);

            return patronDays.Values.Select(patronDay =>
            {
                var dayMonth = $"{patronDay.ElementAt(0)}".Split("/");

                return new PatronDay
                {
                    Day = new DateTime(DateTime.Now.Year, Convert.ToInt32(dayMonth.ElementAt(0)),
                        Convert.ToInt32(dayMonth.ElementAt(1))),
                    CountryCode = $"{patronDay.ElementAt(1)}"
                };
            });
        }

        public async Task<IEnumerable<TeamMate>> GetTeamMates()
        {
            var teamMates = await _spreadSheetService.Get(_options.SpreadsheetId, _options.TeamMatesRange);

            return teamMates.Values.Select(teamMate => new TeamMate
            {
                Id = teamMate.ElementAt(1) as string,
                Name = teamMate.ElementAt(0) as string
            });
        }

        public Task ClearCalendar() =>
            _spreadSheetService.Clear(_options.SpreadsheetId, _options.CalendarRange);

        public Task WriteCalendar(IEnumerable<IEnumerable<object>> values) =>
            _spreadSheetService.Update(_options.SpreadsheetId, _options.CalendarRange,
                values);
    }
}