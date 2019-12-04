namespace SlackAlertOwner.Notifier
{
    using Microsoft.Extensions.Logging;
    using Quartz;
    using System.Threading.Tasks;

    public class NotifyJob : IJob
    {
        readonly ISlackHttpClient _httpClient;
        readonly ILogger<NotifyJob> _logger;

        public NotifyJob(ILogger<NotifyJob> logger, ISlackHttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Start NotifyJob");

            await _httpClient.Notify();
            
            _logger.LogInformation("NotifyJob Completed");

        }
    }
}