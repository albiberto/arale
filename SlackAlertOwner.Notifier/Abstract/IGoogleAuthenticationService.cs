namespace SlackAlertOwner.Notifier.Abstract
{
    using Google.Apis.Sheets.v4;

    public interface IGoogleAuthenticationService
    {
        SheetsService GetService();
    }
}