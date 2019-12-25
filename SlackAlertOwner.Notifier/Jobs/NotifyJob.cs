namespace SlackAlertOwner.Notifier.Jobs
{
    using Abstract;
    using Microsoft.Extensions.Logging;
    using Model;
    using Quartz;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [DisallowConcurrentExecution]
    public class NotifyJob : IJob
    {
        readonly ILogger<NotifyJob> _logger;
        readonly ISlackHttpClient _slackHttpClient;
        readonly IAlertOwnerService _alertOwnerService;

        public NotifyJob(ILogger<NotifyJob> logger, ISlackHttpClient slackHttpClient,
            IAlertOwnerService alertOwnerService)
        {
            _logger = logger;
            _slackHttpClient = slackHttpClient;
            _alertOwnerService = alertOwnerService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            static object BuildRequest(string text)
            {
                var request1 = new
                {
                    text = text
                };
                return request1;
            }

            _logger.LogInformation("Start NotifyJob");

            var teamMates = await _alertOwnerService.GetTeamMates();
            var (today, tomorrow) = await _alertOwnerService.GetShift(teamMates);


            var request1 =
                BuildRequest($"Ciao <@{today.TeamMate.Id}> oggi e' il {today.Schedule.ToString()} ed e' tuo turno.");
            var request2 =
                BuildRequest(
                    $"Ciao <@{tomorrow.TeamMate.Id}> domani e' il {tomorrow.Schedule.ToString()} e sara' il tuo turno.");

            var requests = new List<object>
            {
                request1, request2
            };

        await _slackHttpClient.Notify(requests);

            _logger.LogInformation("NotifyJob Completed");
        }
    }
}