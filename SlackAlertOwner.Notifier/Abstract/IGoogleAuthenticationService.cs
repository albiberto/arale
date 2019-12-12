namespace SlackAlertOwner.Notifier.Abstract
{
    using Google.Apis.Auth.OAuth2;

    public interface IGoogleAuthenticationService
    {
        public UserCredential Authenticate();
    }
}