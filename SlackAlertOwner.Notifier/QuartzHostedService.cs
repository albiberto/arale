namespace SlackAlertOwner.Notifier
{
    using Abstract;
    using Microsoft.Extensions.Hosting;
    using Model;
    using Quartz;
    using Quartz.Spi;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class QuartzHostedService : BackgroundService
    {
        readonly IJobFactory _jobFactory;
        readonly IEnumerable<JobSchedule> _jobSchedules;
        readonly ILoggerAdapter<QuartzHostedService> _logger;
        readonly ISchedulerFactory _schedulerFactory;
        IScheduler Scheduler { get; set; }

        public QuartzHostedService(
            ISchedulerFactory schedulerFactory,
            IJobFactory jobFactory,
            IEnumerable<JobSchedule> jobSchedules,
            ILoggerAdapter<QuartzHostedService> logger)
        {
            _schedulerFactory = schedulerFactory;
            _jobSchedules = jobSchedules;
            _logger = logger;
            _jobFactory = jobFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("QuartzHostedService Start");

            await RunAsync(stoppingToken);
            await Task.Delay(-1, stoppingToken);
            await KillAsync(stoppingToken);

            _logger.LogInformation("QuartzHostedService Shutdown");
        }

        async Task RunAsync(CancellationToken cancellationToken)
        {
            Scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
            Scheduler.JobFactory = _jobFactory;

            foreach (var jobSchedule in _jobSchedules)
            {
                var job = CreateJob(jobSchedule);
                var trigger = CreateTrigger(jobSchedule);

                await Scheduler.ScheduleJob(job, trigger, cancellationToken);
            }

            await Scheduler.Start(cancellationToken);
        }

        async Task KillAsync(CancellationToken cancellationToken)
        {
            if (Scheduler != null) await Scheduler?.Shutdown(cancellationToken);
        }

        static IJobDetail CreateJob(JobSchedule schedule)
        {
            var jobType = schedule.JobType;
            return JobBuilder
                .Create(jobType)
                .WithIdentity(jobType.FullName)
                .WithDescription(jobType.Name)
                .Build();
        }

        static ITrigger CreateTrigger(JobSchedule schedule) =>
            TriggerBuilder
                .Create()
                .WithIdentity($"{schedule.JobType.FullName}.trigger")
                .WithCronSchedule(schedule.CronExpression)
                .WithDescription(schedule.CronExpression)
                .Build();
    }
}