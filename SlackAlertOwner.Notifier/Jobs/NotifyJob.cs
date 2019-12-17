namespace SlackAlertOwner.Notifier.Jobs
{
    using Abstract;
    using Microsoft.Extensions.Logging;
    using Quartz;
    using System.Threading.Tasks;

    [DisallowConcurrentExecution]
    public class NotifyJob : IJob
    {
        readonly ILogger<NotifyJob> _logger;
        readonly ISlackHttpClient _slackHttpClient;
        readonly ISpreadSheetService _spreadSheetService;

        public NotifyJob(ILogger<NotifyJob> logger, ISlackHttpClient slackHttpClient,
            ISpreadSheetService spreadSheetService)
        {
            _logger = logger;
            _slackHttpClient = slackHttpClient;
            _spreadSheetService = spreadSheetService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Start NotifyJob");

            var teamMates = await _spreadSheetService.GetTeamMates();
            var shift = await _spreadSheetService.GetShift(teamMates);

            var request = new
            {
                text = $"Ciao <@{shift.TeamMate.Id}> oggi e' il {shift.Schedule.ToString()} ed e' tuo turno."
            };

            await _slackHttpClient.Notify(request);

            _logger.LogInformation("NotifyJob Completed");
        }
    }
}