namespace SlackAlertOwner.Notifier.Jobs
{
    using Quartz;
    using System.Threading.Tasks;

    public class CalendarJob : IJob
    {
        public Task Execute(IJobExecutionContext context) => throw new System.NotImplementedException();
    }
}