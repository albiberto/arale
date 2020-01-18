namespace SlackAlertOwner.Tests
{
    using Microsoft.Extensions.Logging;
    using Moq;
    using NodaTime;
    using Notifier.Abstract;
    using Notifier.Jobs;
    using Notifier.Model;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [TestFixture]
    public class NotifyJobTests
    {
        [SetUp]
        public void Setup()
        {
            _repository = new MockRepository(MockBehavior.Strict) {DefaultValue = DefaultValue.Mock};

            _slackHttpClient = _repository.Create<ISlackHttpClient>();

            _alertOwnerService = _repository.Create<IAlertOwnerService>();
            _alertOwnerService
                .Setup(s => s.GetTeamMates())
                .ReturnsAsync(_teamMates);
            
            var logger = _repository.Create<ILoggerAdapter<NotifyJob>>();
            logger.Setup(s => s.LogInformation(It.IsAny<string>()));

            _logger = logger.Object;
        }

        [TearDown]
        public void VerifyAndTearDown()
        {
            _repository.VerifyAll();
        }

        MockRepository _repository;

        Mock<ISlackHttpClient> _slackHttpClient;
        Mock<IAlertOwnerService> _alertOwnerService;
        ILoggerAdapter<NotifyJob> _logger;

        readonly IEnumerable<TeamMate> _teamMates = new List<TeamMate>
        {
            new TeamMate("1", "IronMan", "US"),
            new TeamMate("2", "Hulk", "LA"),
            new TeamMate("3", "Thor", "AZ")
        };

        readonly TeamMate _ironMan = new TeamMate("1", "IronMan", "US");
        readonly TeamMate _hulk = new TeamMate("2", "Hulk", "LA");

        [Test]
        public async Task Should_notify_in_the_middle_of_the_week()
        {
            var today = new Shift(_ironMan, new LocalDate(2019, 12, 3));
            var tomorrow = new Shift(_hulk, new LocalDate(2019, 12, 4));

            _alertOwnerService
                .Setup(s =>
                    s.GetShift(It.Is<IEnumerable<TeamMate>>(tm => Equals(tm, _teamMates))))
                .ReturnsAsync((today, tomorrow));

            _slackHttpClient
                .Setup(s => s.Notify(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var sut = new NotifyJob(_slackHttpClient.Object, _alertOwnerService.Object, _logger);
            await sut.Execute(null);

            _slackHttpClient
                .Verify(v => v.Notify(It.IsAny<string>()), Times.Exactly(2));
        }
        
        [Test]
        public async Task Should_not_notify_in_the_weekend()
        {
            _alertOwnerService
                .Setup(s =>
                    s.GetShift(It.Is<IEnumerable<TeamMate>>(tm => Equals(tm, _teamMates))))
                .ReturnsAsync((null, null));

            var sut = new NotifyJob(_slackHttpClient.Object, _alertOwnerService.Object, _logger);
            await sut.Execute(null);

            _slackHttpClient
                .Verify(v => v.Notify(It.IsAny<string>()), Times.Never);
        }
        
        [Test]
        public async Task Should_notify_in_only_one_at_friday()
        {
            var today = new Shift(_ironMan, new LocalDate(2019, 12, 3));

            _alertOwnerService
                .Setup(s =>
                    s.GetShift(It.Is<IEnumerable<TeamMate>>(tm => Equals(tm, _teamMates))))
                .ReturnsAsync((today, null));
            
            _slackHttpClient
                .Setup(s => s.Notify(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var sut = new NotifyJob(_slackHttpClient.Object, _alertOwnerService.Object, _logger);
            await sut.Execute(null);

            _slackHttpClient
                .Verify(v => v.Notify(It.IsAny<string>()), Times.Once);
        }
    }
}