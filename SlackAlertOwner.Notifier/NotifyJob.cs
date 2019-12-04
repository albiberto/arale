namespace SlackAlertOwner.Notifier
{
    using Microsoft.Extensions.Logging;
    using Quartz;
    using System.Threading.Tasks;

    public class NotifyJob : IJob
    {
        private readonly ILogger<NotifyJob> _logger;
        public NotifyJob(ILogger<NotifyJob> logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Start NotifyJob");
            return Task.CompletedTask;
        }
    }
}