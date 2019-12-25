namespace SlackAlertOwner.Notifier.Jobs
{
    using Abstract;
    using Microsoft.Extensions.Logging;
    using NodaTime;
    using Quartz;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [DisallowConcurrentExecution]
    public class NotifyJob : IJob
    {
        readonly IAlertOwnerSpreadServiceService _alertOwnerSpreadServiceService;
        readonly ITypeConverter<LocalDate> _converter;
        readonly ILogger<NotifyJob> _logger;
        readonly ISlackHttpClient _slackHttpClient;

        public NotifyJob(
            ISlackHttpClient slackHttpClient,
            IAlertOwnerSpreadServiceService alertOwnerSpreadServiceService,
            ITypeConverter<LocalDate> converter,
            ILogger<NotifyJob> logger 
            )
        {
            _slackHttpClient = slackHttpClient;
            _alertOwnerSpreadServiceService = alertOwnerSpreadServiceService;
            _converter = converter;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Start NotifyJob");

            var teamMates = await _alertOwnerSpreadServiceService.GetTeamMates();
            var (today, tomorrow) = await _alertOwnerSpreadServiceService.GetShift(teamMates);

            var requests = new List<string>
            {
                $"Ciao <@{tomorrow.TeamMate.Id}> domani e' il {tomorrow.Schedule.ToString()} e sara' il tuo turno.",
                $"Ciao <@{today.TeamMate.Id}> oggi e' il {today.Schedule.ToString()} ed e' tuo turno."
            };

            await _slackHttpClient.Notify(requests);

            _logger.LogInformation("NotifyJob Completed");
        }
    }
}