namespace SlackAlertOwner.Notifier.Clients
{
    using Abstract;
    using Google.Apis.Sheets.v4;
    using Google.Apis.Sheets.v4.Data;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class GoogleSpreadSheetClient : IGoogleSpreadSheetClient
    {
        readonly ISheetsServiceFactory _authServiceFactory;

        public GoogleSpreadSheetClient(ISheetsServiceFactory authServiceFactory)
        {
            _authServiceFactory = authServiceFactory;
        }

        public async Task<ValueRange> Get(string id, string range)
        {
            using var service = _authServiceFactory.Build();
            var request = service.Spreadsheets.Values.Get(id, range);

            return await request.ExecuteAsync();
        }

        public async Task<ClearValuesResponse> Clear(string id, string range, ClearValuesRequest requestBody = default)
        {
            requestBody ??= new ClearValuesRequest();

            using var service = _authServiceFactory.Build();
            var request = service.Spreadsheets.Values.Clear(requestBody, id, range);

            return await request.ExecuteAsync();
        }

        public async Task<UpdateValuesResponse> Update(string id, string range, IEnumerable<IEnumerable<object>> values)
        {
            var requestBody = new ValueRange
            {
                Values = values.Select(Enumerable.ToList).ToList<IList<object>>()
            };

            using var service = _authServiceFactory.Build();
            var request = service.Spreadsheets.Values.Update(requestBody, id, range);
            
            request.ValueInputOption =
                (SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum?) SpreadsheetsResource
                    .ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;

            return await request.ExecuteAsync();
        }
    }
}