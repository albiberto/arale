namespace SlackAlertOwner.Notifier.Jobs
{
    using Abstract;
    using Quartz;
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
            static string GetRegard()
            {
                var regards = new List<string> {"Hola", "Hello", "Ciao"};
                return regards.ElementAt(new Random().Next(0, regards.Count - 1));
            }

            _logger.Log("Start NotifyJob");

            var teamMates = await _alertOwnerService.GetTeamMates();
            var (today, tomorrow) = await _alertOwnerService.GetShift(teamMates);

            var requests = new List<string>
            {
                @$"{GetRegard()} <@{today.TeamMate.Id}>. Today is your shift!",
                @$"{GetRegard()} <@{tomorrow.TeamMate.Id}>. Tomorrow will be your shift!"
            };

            await _slackHttpClient.Notify(requests);

            _logger.Log("NotifyJob Completed");
        }
    }
}