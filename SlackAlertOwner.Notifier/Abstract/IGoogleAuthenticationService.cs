namespace SlackAlertOwner.Notifier.Abstract
{
    using Google.Apis.Auth.OAuth2;
    using System.Threading.Tasks;

    public interface IGoogleAuthenticationService
    {
        public Task<UserCredential> Authenticate();
    }
}