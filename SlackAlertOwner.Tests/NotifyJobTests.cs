namespace SlackAlertOwner.Tests
{
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
            _logger = _repository.Create<ILogService>();
        }

        [TearDown]
        public void VerifyAndTearDown()
        {
            _repository.VerifyAll();
        }

        MockRepository _repository;

        Mock<ISlackHttpClient> _slackHttpClient;
        Mock<IAlertOwnerService> _alertOwnerService;
        Mock<ILogService> _logger;

        readonly IEnumerable<TeamMate> _teamMates = new List<TeamMate>
        {
            new TeamMate("1", "IronMan", "US"),
            new TeamMate("2", "Hulk", "LA"),
            new TeamMate("3", "Thor", "AZ")
        };

        [Test]
        public async Task Should_run_NotifyJob()
        {
            _alertOwnerService
                .Setup(s => s.GetTeamMates())
                .ReturnsAsync(_teamMates);

            var ironMan = new TeamMate("1", "IronMan", "US");
            var hulk = new TeamMate("2", "Hulk", "LA");

            var today = new Shift(ironMan, new LocalDate(2019, 12, 1));
            var tomorrow = new Shift(hulk, new LocalDate(2019, 12, 2));
            var shifts = (today, tomorrow);

            _alertOwnerService
                .Setup(s =>
                    s.GetShift(It.Is<IEnumerable<TeamMate>>(tm => Equals(tm, _teamMates))))
                .ReturnsAsync(shifts);

            _slackHttpClient
                .Setup(s => s.Notify(It.IsAny<List<string>>()))
                .Returns(Task.CompletedTask);

            _logger.Setup(s => s.Log(It.IsAny<string>()));

            var sut = new NotifyJob(_slackHttpClient.Object, _alertOwnerService.Object, _logger.Object);

            await sut.Execute(null);
        }
    }
}