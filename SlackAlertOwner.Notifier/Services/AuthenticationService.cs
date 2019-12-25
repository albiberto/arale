namespace SlackAlertOwner.Notifier.Services
{
    using Abstract;
    using Google.Apis.Auth.OAuth2;
    using Google.Apis.Services;
    using Google.Apis.Sheets.v4;
    using Microsoft.Extensions.Options;
    using Model;
    using System.Security.Cryptography.X509Certificates;

    public class AuthenticationService : IAuthenticationService
    {
        readonly MyOptions _options;

        public AuthenticationService(IOptions<MyOptions> options)
        {
            _options = options.Value;
        }

        public SheetsService GetService()
        {
            var certificate =
                new X509Certificate2(_options.Certificate, _options.Password, X509KeyStorageFlags.Exportable);

            var credential = new ServiceAccountCredential(
                new ServiceAccountCredential.Initializer(_options.ServiceAccountEmail)
                {
                    Scopes = new[] {SheetsService.Scope.Spreadsheets}
                }.FromCertificate(certificate));


            return new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = _options.ApplicationName
            });
        }
    }
}