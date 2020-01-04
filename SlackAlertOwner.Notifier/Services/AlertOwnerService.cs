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

    public class AlertOwnerService : IAlertOwnerService
    {
        readonly IGoogleSpreadSheetClient _googleSpreadSheetClient;
        readonly ITypeConverter<LocalDate> _converter;
        readonly ITimeService _timeService;
        readonly MyOptions _options;

        public AlertOwnerService(IGoogleSpreadSheetClient googleSpreadSheetClient,
            ITypeConverter<LocalDate> converter, ITimeService timeService, IOptions<MyOptions> options)
        {
            _googleSpreadSheetClient = googleSpreadSheetClient;
            _converter = converter;
            _timeService = timeService;
            _options = options.Value;
        }

        public async Task<(Shift today, Shift tomorrow)> GetShift(IEnumerable<TeamMate> teamMates)
        {
            var shiftsCalendar = await _googleSpreadSheetClient.Get(_options.SpreadsheetId, _options.CalendarRange);

            var now = _timeService.Now;

            var result = (from shift in shiftsCalendar.Values
                    let schedule = _converter.ParseValueFromString(shift.ElementAt(0) as string)
                    let teamMate = $"{shift.ElementAt(1)}"
                    where schedule == now || schedule == now.PlusDays(1)
                    orderby schedule
                    select new Shift(new TeamMate(teamMates.First(tm => tm.Name.Contains(teamMate)).Id, teamMate, null),
                        schedule)
                ).ToList();

            var today = result.FirstOrDefault();
            var tomorrow = result.Skip(1).FirstOrDefault();

            return (today, tomorrow);
        }
        
        public async Task<IEnumerable<TeamMate>> GetTeamMates() =>
            from teamMate in (await _googleSpreadSheetClient.Get(_options.SpreadsheetId, _options.TeamMatesRange))
                .Values
            let id = teamMate.ElementAt(1) as string
            let name = teamMate.ElementAt(0) as string
            let countryCode = teamMate.ElementAt(2) as string
            select new TeamMate(id, name, countryCode);

        public async Task<IEnumerable<PatronDay>> GetPatronDays()
        {
            var patronDays = await _googleSpreadSheetClient.Get(_options.SpreadsheetId, _options.PatronDaysRange);

            var result = patronDays.Values.Select(patronDay =>
            {
                var dayMonth = $"{patronDay.ElementAt(0)}".Split("/");

                return new PatronDay(
                    new LocalDate(_timeService.Now.Year, Convert.ToInt32(dayMonth.ElementAt(1)),
                        Convert.ToInt32(dayMonth.ElementAt(0))),
                    $"{patronDay.ElementAt(1)}"
                );
            });

            return result;
        }

        public async Task<IEnumerable<Shift>> GetCalendar(IEnumerable<TeamMate> teamMates) =>
            from calendar in (await _googleSpreadSheetClient.Get(_options.SpreadsheetId, _options.CalendarRange)).Values
            let date = _converter.ParseValueFromString(calendar.ElementAt(0) as string)
            let mate = teamMates.First(tm =>
                string.Equals(tm.Name, calendar.ElementAt(1) as string, StringComparison.InvariantCulture))
            select new Shift(mate, date);

        public Task ClearCalendar() =>
            _googleSpreadSheetClient.Clear(_options.SpreadsheetId, _options.CalendarRange);

        public Task WriteCalendar(IEnumerable<IEnumerable<object>> values) =>
            _googleSpreadSheetClient.Update(_options.SpreadsheetId, _options.CalendarRange,
                values);
    }
}