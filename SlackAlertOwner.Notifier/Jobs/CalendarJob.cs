namespace SlackAlertOwner.Notifier.Jobs
{
    using Abstract;
    using Microsoft.Extensions.Logging;
    using Quartz;
    using System.Threading.Tasks;

    [DisallowConcurrentExecution]
    public class CalendarJob : IJob
    {
        readonly ILogger<NotifyJob> _logger;
        readonly ISpreadSheetService _spreadSheetService;

        public CalendarJob(ISpreadSheetService spreadSheetService, ILogger<NotifyJob> logger)
        {
            _spreadSheetService = spreadSheetService;
            _logger = logger;
        }
        
        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Start CalendarJob");

            await _spreadSheetService.ClearCalendar();

            var teamMates = await _spreadSheetService.GetTeamMates();
            var patronDays = await _spreadSheetService.GetPatronDays();
            
            await _spreadSheetService.WriteCalendar(teamMates);
            
            _logger.LogInformation("CalendarJob Completed");
        }
    }
}