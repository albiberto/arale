namespace SlackAlertOwner.Notifier.Jobs
{
    using Abstract;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Model;
    using Quartz;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class NotifyJob : IJob
    {
        readonly ISlackHttpClient _slackHttpClient;
        readonly ISpreadSheetService _spreadSheetService;
        readonly IEnumerable<TeamMate> _teamMates;
        readonly ILogger<NotifyJob> _logger;

        public NotifyJob(ILogger<NotifyJob> logger, ISlackHttpClient slackHttpClient, ISpreadSheetService spreadSheetService, IOptions<MyOptions> options)
        {
            _logger = logger;
            _slackHttpClient = slackHttpClient;
            _spreadSheetService = spreadSheetService;
            _teamMates = options.Value.TeamMates;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Start NotifyJob");
         
            var shift = await _spreadSheetService.Read();

            shift.TeamMate.Id = _teamMates.First(mate => mate.Name.Contains(shift.TeamMate.Name)).Id;
            
            var request = new
            {
                text = $"Ciao <@{shift.TeamMate.Id}> oggi e' il {shift.Schedule.ToString()} ed e' tuo turno."
            };
            
            await _slackHttpClient.Notify(request);
            
            _logger.LogInformation("NotifyJob Completed");
        }
    }
}