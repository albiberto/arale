namespace SlackAlertOwner.Notifier
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Model;
    using Quartz;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class NotifyJob : IJob
    {
        readonly ISlackHttpClient _httpClient;
        readonly IEnumerable<TeamMate> _teamMates;
        readonly ILogger<NotifyJob> _logger;

        public NotifyJob(ILogger<NotifyJob> logger, ISlackHttpClient httpClient, IOptions<MyOptions> options)
        {
            _logger = logger;
            _httpClient = httpClient;
            _teamMates = options.Value.TeamMates;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Start NotifyJob");

            foreach (var teamMate in _teamMates)
            {
                var request = new
                {
                    text = $"Ciao <@{teamMate.Id}>"
                };
                
                await _httpClient.Notify(request);
            }
            
            _logger.LogInformation("NotifyJob Completed");
        }
    }
}