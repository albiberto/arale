namespace SlackAlertOwner.Notifier.Abstract
{
    public interface ILogService
    {
        void Log(string text);
        void Error(string text);
    }
}