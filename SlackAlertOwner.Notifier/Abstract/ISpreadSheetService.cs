namespace SlackAlertOwner.Notifier.Abstract
{
    using Google.Apis.Sheets.v4.Data;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ISpreadSheetService
    {
        Task<ValueRange> Get(string id, string range);
        Task<ClearValuesResponse> Clear(string id, string range, ClearValuesRequest requestBody = default);
        Task<UpdateValuesResponse> Update(string id, string range, IEnumerable<IEnumerable<object>> values);
    }
}