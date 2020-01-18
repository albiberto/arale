namespace SlackAlertOwner.Notifier.Jobs
{
    using Abstract;
    using Microsoft.Extensions.Options;
    using Model;
    using Quartz;
    using System;
    using System.Threading.Tasks;

    public class WelcomeJob : IJob
    {
        readonly ISlackHttpClient _client;
        readonly ILoggerAdapter<WelcomeJob> _logger;
        readonly MyOptions _options;

        public WelcomeJob(ISlackHttpClient client, IOptions<MyOptions> options, ILoggerAdapter<WelcomeJob> logger)
        {
            _client = client;
            _logger = logger;
            _options = options.Value;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Start WelcomeJob");

            try
            {
                await _client.Notify(
                    $@"Hi <!channel>. I'm {_options.ApplicationName}, I am your shifts managing assistant!");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{e.Message}");
            }

            _logger.LogInformation("WelcomeJob Completed");
        }
    }
}