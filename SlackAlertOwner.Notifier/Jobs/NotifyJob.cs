namespace SlackAlertOwner.Notifier.Jobs
{
    using Abstract;
    using Quartz;
    using Services;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [DisallowConcurrentExecution]
    public class NotifyJob : IJob
    {
        readonly IAlertOwnerService _alertOwnerService;
        readonly ILogService _logger;
        readonly ISlackHttpClient _slackHttpClient;

        public NotifyJob(
            ISlackHttpClient slackHttpClient,
            IAlertOwnerService alertOwnerService,
            ILogService logger
        )
        {
            _slackHttpClient = slackHttpClient;
            _alertOwnerService = alertOwnerService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.Log("Start NotifyJob");

            var teamMates = await _alertOwnerService.GetTeamMates();
            var (today, tomorrow) = await _alertOwnerService.GetShift(teamMates);

            var requests = new List<string>
            {
                MessageService.Today(today.TeamMate),
                MessageService.Tomorrow(tomorrow.TeamMate)
            };

            await _slackHttpClient.Notify(requests);

            _logger.Log("NotifyJob Completed");
        }
    }
}