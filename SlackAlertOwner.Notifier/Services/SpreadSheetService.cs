namespace SlackAlertOwner.Notifier.Services
{
    using Abstract;
    using Google.Apis.Services;
    using Google.Apis.Sheets.v4;
    using Microsoft.Extensions.Options;
    using Model;
    using NodaTime;
    using System;
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

        public async Task<Shift> Read()
        {
            var service = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = await _authenticationService.Authenticate(),
                ApplicationName = _options.ApplicationName
            });

            var request = service.Spreadsheets.Values.Get(_options.SpreadsheetId, _options.SpreadsheetRange);
            var response = await request.ExecuteAsync();

            var now = LocalDate.FromDateTime(DateTime.Now);

            return (from value in response.Values
                    let schedule = _converter.ParseValueFromString(value.ElementAt(1) as string)
                    where schedule == now
                    select new Shift
                    {
                        Schedule = schedule,
                        TeamMate = new TeamMate
                        {
                            Name = $"{value.ElementAt(0)}"
                        }
                    }
                ).FirstOrDefault();
        }
    }
}