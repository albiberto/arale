namespace SlackAlertOwner.Notifier.Abstract
{
    using Google.Apis.Sheets.v4;

    public interface IAuthenticationService
    {
        SheetsService GetService();
    }
}