namespace SlackAlertOwner.Notifier.Services
{
    using Abstract;
    using Microsoft.Extensions.Logging;

    public class LogService : ILogService
    {
        readonly ILogger _logger;

        public LogService(ILogger logger)
        {
            _logger = logger;
        }

        public void Log(string text) => _logger.LogInformation(text);
    }
}