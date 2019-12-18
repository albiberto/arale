namespace SlackAlertOwner.Notifier.Services
{
    using Abstract;
    using Google.Apis.Services;
    using Google.Apis.Sheets.v4;
    using Google.Apis.Sheets.v4.Data;
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
        readonly IShiftsCalendarService _shiftsCalendarService;
        readonly MyOptions _options;

        public SpreadSheetService(IGoogleAuthenticationService authenticationService, IOptions<MyOptions> options,
            IShiftsCalendarService shiftsCalendarService, ITypeConverter<LocalDate> converter)
        {
            _authenticationService = authenticationService;
            _shiftsCalendarService = shiftsCalendarService;
            _converter = converter;
            _options = options.Value;
        }

        public async Task<Shift> GetShift(IEnumerable<TeamMate> teamMates)
        {
            var service = await GetService();

            var shiftsCalendar = (await service.Spreadsheets.Values.Get(_options.SpreadsheetId, _options.CalendarRange)
                .ExecuteAsync()).Values;

            var now = LocalDate.FromDateTime(DateTime.Now);

            return (from shift in shiftsCalendar
                    let schedule = _converter.ParseValueFromString(shift.ElementAt(1) as string)
                    let teamMate = $"{shift.ElementAt(0)}"
                    where schedule == now
                    select new Shift (new TeamMate
                    {
                        Id = teamMates.First(tm => tm.Name.Contains(teamMate)).Id,
                        Name = teamMate
                    }, schedule)
                ).First();
        }

        public async Task WriteCalendar(IEnumerable<TeamMate> teamMates)
        {
            var service = await GetService();

            teamMates = teamMates.OrderBy(teamMate => teamMate.Name.Split(" ").ElementAt(1));
            var patronDays = await GetPatronDays();


            _shiftsCalendarService.MonthCalendar(patronDays, teamMates);

            // var requestBody = new ValueRange
            // {
            //     Values = new List<IList<object>>
            //     {
            //         calendar
            //     }
            // };

            // var requestBody = new ValueRange()
            // {
            //     Values = new List<IList<object>>()
            //     {
            //         new List<object>()
            //         {
            //             "ciao", "alberto"
            //         },
            //         new List<object>()
            //         {
            //             "hello", "roald"
            //         }
            //     }
            // };

            // SpreadsheetsResource.ValuesResource.UpdateRequest request = service.Spreadsheets.Values.Update(requestBody, _options.SpreadsheetId, _options.CalendarRange);
            // request.ValueInputOption = (SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum?) SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
            //
            // await request.ExecuteAsync();
        }

        public async Task ClearCalendar()
        {
            var service = await GetService();

            var requestBody = new ClearValuesRequest();
            var request =
                service.Spreadsheets.Values.Clear(requestBody, _options.SpreadsheetId, _options.CalendarRange);

            await request.ExecuteAsync();
        }

        public async Task<IEnumerable<PatronDay>> GetPatronDays()
        {
            var service = await GetService();

            return (await service.Spreadsheets.Values.Get(_options.SpreadsheetId, _options.PatronDaysRange)
                .ExecuteAsync()).Values.Select(patronDay =>
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
            var service = await GetService();

            return (await service.Spreadsheets.Values.Get(_options.SpreadsheetId, _options.TeamMatesRange)
                .ExecuteAsync()).Values.Select(teamMate => new TeamMate
            {
                Id = teamMate.ElementAt(1) as string,
                Name = teamMate.ElementAt(0) as string
            });
        }

        async Task<SheetsService> GetService() =>
            new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = await _authenticationService.Authenticate(),
                ApplicationName = _options.ApplicationName
            });
    }
}