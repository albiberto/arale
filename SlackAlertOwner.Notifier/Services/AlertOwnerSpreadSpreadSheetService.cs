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
        readonly IGoogleSpreadSheetService _googleSpreadSheetService;
        readonly MyOptions _options;

        public AlertOwnerSpreadSpreadSheetService(IGoogleSpreadSheetService googleSpreadSheetService,
            IOptions<MyOptions> options, ITypeConverter<LocalDate> converter)
        {
            _googleSpreadSheetService = googleSpreadSheetService;
            _converter = converter;
            _options = options.Value;
        }

        public async Task<Shift> GetShift(IEnumerable<TeamMate> teamMates)
        {
            var shiftsCalendar = await _googleSpreadSheetService.Get(_options.SpreadsheetId, _options.CalendarRange);

            var now = LocalDate.FromDateTime(DateTime.Now);

            return (from shift in shiftsCalendar.Values
                    let schedule = _converter.ParseValueFromString(shift.ElementAt(1) as string)
                    let teamMate = $"{shift.ElementAt(0)}"
                    where schedule == now
                    select new Shift(new TeamMate(teamMates.First(tm => tm.Name.Contains(teamMate)).Id, teamMate, "TS"),
                        schedule)
                ).First();
        }

        public async Task<IEnumerable<PatronDay>> GetPatronDays()
        {
            var patronDays = await _googleSpreadSheetService.Get(_options.SpreadsheetId, _options.PatronDaysRange);

            var result = patronDays.Values.Select(patronDay =>
            {
                var dayMonth = $"{patronDay.ElementAt(0)}".Split("/");

                return new PatronDay
                {
                    Day = new LocalDate(DateTime.Now.Year, Convert.ToInt32(dayMonth.ElementAt(1)),
                        Convert.ToInt32(dayMonth.ElementAt(0))),
                    CountryCode = $"{patronDay.ElementAt(1)}"
                };
            });

            return result;
        }

        public async Task<IEnumerable<TeamMate>> GetTeamMates() =>
            from teamMate in (await _googleSpreadSheetService.Get(_options.SpreadsheetId, _options.TeamMatesRange))
                .Values
            let id = teamMate.ElementAt(1) as string
            let name = teamMate.ElementAt(0) as string
            let countryCode = teamMate.ElementAt(2) as string
            select new TeamMate(id, name, countryCode);

        public Task ClearCalendar() =>
            _googleSpreadSheetService.Clear(_options.SpreadsheetId, _options.CalendarRange);

        public Task WriteCalendar(IEnumerable<IEnumerable<object>> values) =>
            _googleSpreadSheetService.Update(_options.SpreadsheetId, _options.CalendarRange,
                values);
    }
}