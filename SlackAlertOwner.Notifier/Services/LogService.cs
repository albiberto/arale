namespace SlackAlertOwner.Notifier.Services
{
    using Abstract;
    using Jobs;
    using Microsoft.Extensions.Logging;

    public class LogService : ILogService
    {
        readonly ILogger<NotifyJob> _logger;

        public LogService(ILogger<NotifyJob> logger)
        {
            _logger = logger;
        }

        public void Log(string text) => _logger.LogInformation(text);
    }
}