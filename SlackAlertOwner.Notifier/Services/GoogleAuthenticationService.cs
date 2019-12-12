namespace SlackAlertOwner.Notifier.Services
{
    using Abstract;
    using Google.Apis.Auth.OAuth2;
    using Google.Apis.Sheets.v4;
    using Google.Apis.Util.Store;
    using Microsoft.Extensions.Options;
    using Model;
    using System.IO;
    using System.Threading;

    public class GoogleAuthenticationService : IGoogleAuthenticationService
    {
        static readonly string[] Scopes = {SheetsService.Scope.SpreadsheetsReadonly};
        readonly MyOptions _options;

        public GoogleAuthenticationService(IOptions<MyOptions> options)
        {
            _options = options.Value;
        }

        public UserCredential Authenticate()
        {
            using var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read);

            // The file stores the user's access and refresh tokens, and is created automatically when the authorization flow completes for the first time.
            return GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(_options.CredPath, true)).Result;
        }
    }
}