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
    using System.Globalization;
    using System.Linq;

    public class SpreadSheetService : ISpreadSheetService
    {
        readonly IGoogleAuthenticationService _authenticationService;
        readonly MyOptions _options;
        readonly IShiftService _shiftService;


        public SpreadSheetService(IGoogleAuthenticationService authenticationService, IOptions<MyOptions> options,
            IShiftService shiftService)
        {
            _authenticationService = authenticationService;
            _shiftService = shiftService;
            _options = options.Value;
        }

        public Shift Read()
        {
            var service = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = _authenticationService.Authenticate(),
                ApplicationName = _options.ApplicationName
            });

            var request = service.Spreadsheets.Values.Get(_options.SpreadsheetId, _options.SpreadsheetRange);
            var response = request.Execute();

            var now = LocalDate.FromDateTime(DateTime.Now);

            return (from value in response.Values
                let schedule = ParseExact(value)
                where new LocalDate(schedule.Year, schedule.Month, schedule.Day) == now
                select _shiftService.Parse(value)).FirstOrDefault();
        }

        DateTime ParseExact(IEnumerable<object> value) => DateTime.ParseExact(
            value.Skip(1).FirstOrDefault()?.ToString() ?? string.Empty, _options.Pattern, CultureInfo.InvariantCulture);
    }
}