namespace SlackAlertOwner.Notifier.Services
{
    using Abstract;
    using Google.Apis.Services;
    using Google.Apis.Sheets.v4;
    using Microsoft.Extensions.Options;
    using Model;
    using NodaTime;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class SpreadSheetService : ISpreadSheetService
    {
        readonly IGoogleAuthenticationService _authenticationService;
        readonly ITypeConverter<LocalDate> _converter;
        readonly MyOptions _options;


        public SpreadSheetService(IGoogleAuthenticationService authenticationService, IOptions<MyOptions> options,
            ITypeConverter<LocalDate> converter)
        {
            _authenticationService = authenticationService;
            _converter = converter;
            _options = options.Value;
        }

        public async Task<Shift> GetShift()
        {
            var service = await GetService();

            var shiftsCalendar = (await service.Spreadsheets.Values.Get(_options.SpreadsheetId, _options.CalendarRange)
                .ExecuteAsync()).Values;

            var teamMates = await GetTeamMates();

            var now = LocalDate.FromDateTime(DateTime.Now);

            return (from shift in shiftsCalendar
                    let schedule = _converter.ParseValueFromString(shift.ElementAt(1) as string)
                    let teamMate = $"{shift.ElementAt(0)}"
                    where schedule == now
                    select new Shift
                    {
                        Schedule = schedule,
                        TeamMate = new TeamMate
                        {
                            Id = teamMates.First(tm => tm.Name.Contains(teamMate)).Id,
                            Name = teamMate
                        }
                    }
                ).First();
        }

        public async Task WriteCalendar()
        {
            var service = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = await _authenticationService.Authenticate(),
                ApplicationName = _options.ApplicationName
            });

            var request = service.Spreadsheets.Values.Get(_options.SpreadsheetId, _options.CalendarRange);
            var response = await request.ExecuteAsync();
        }

        async Task<SheetsService> GetService()
        {
            var service = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = await _authenticationService.Authenticate(),
                ApplicationName = _options.ApplicationName
            });
            return service;
        }

        async Task<IEnumerable<TeamMate>> GetTeamMates()
        {
            var service = await GetService();

            return (await service.Spreadsheets.Values.Get(_options.SpreadsheetId, _options.TeamMatesRange)
                .ExecuteAsync()).Values.Select(teamMate => new TeamMate
            {
                Id = teamMate.ElementAt(1) as string,
                Name = teamMate.ElementAt(0) as string
            });
        }
    }
}