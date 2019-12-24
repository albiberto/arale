namespace SlackAlertOwner.Notifier.Services
{
    using Abstract;
    using Google.Apis.Sheets.v4;
    using Google.Apis.Sheets.v4.Data;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class GoogleGoogleSpreadSheetService : IGoogleSpreadSheetService
    {
        readonly SheetsService _service;

        public GoogleGoogleSpreadSheetService(IGoogleAuthenticationService authService)
        {
            _service = authService.GetService();
        }

        public async Task<ValueRange> Get(string id, string range)
        {
            var request = _service.Spreadsheets.Values.Get(id, range);

            return await request.ExecuteAsync();
        }

        public async Task<ClearValuesResponse> Clear(string id, string range, ClearValuesRequest requestBody = default)
        {
            requestBody ??= new ClearValuesRequest();

            var request = _service.Spreadsheets.Values.Clear(requestBody, id, range);

            return await request.ExecuteAsync();
        }

        public async Task<UpdateValuesResponse> Update(string id, string range, IEnumerable<IEnumerable<object>> values)
        {
            var requestBody = new ValueRange
            {
                Values = values.Select(Enumerable.ToList).ToList<IList<object>>()
            };

            var request = _service.Spreadsheets.Values.Update(requestBody, id, range);
            request.ValueInputOption =
                (SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum?) SpreadsheetsResource
                    .ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;

            return await request.ExecuteAsync();
        }
    }
}