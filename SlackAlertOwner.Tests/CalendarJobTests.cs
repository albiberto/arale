namespace SlackAlertOwner.Tests
{
    using Moq;
    using NodaTime;
    using Notifier.Abstract;
    using Notifier.Jobs;
    using Notifier.Model;
    using Notifier.Services;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [TestFixture]
    public class CalendarJobTests
    {
        [SetUp]
        public void Setup()
        {
            _repository = new MockRepository(MockBehavior.Strict) {DefaultValue = DefaultValue.Mock};

            _slackHttpClient = _repository.Create<ISlackHttpClient>();
            _alertOwnerService = _repository.Create<IAlertOwnerService>();
            _shiftsService = _repository.Create<IShiftsService>();

            var logger = _repository.Create<ILogService>();
            logger
                .Setup(s => s.Log(It.IsAny<string>()));

            _logger = logger.Object;

            _converter = _repository.Create<ITypeConverter<LocalDate>>()
                .Object;
        }

        [TearDown]
        public void VerifyAndTearDown()
        {
            _repository.VerifyAll();
        }

        Mock<ISlackHttpClient> _slackHttpClient;
        Mock<IAlertOwnerService> _alertOwnerService;
        Mock<IShiftsService> _shiftsService;
        ITypeConverter<LocalDate> _converter;
        ILogService _logger;

        MockRepository _repository;

        static readonly TeamMate Hero1 = new TeamMate("1", "IronMan", "US");
        static readonly TeamMate Hero2 = new TeamMate("2", "Hulk", "LA");
        static readonly TeamMate Hero3 = new TeamMate("3", "Thor", "AZ");


        readonly IEnumerable<TeamMate> _teamMates = new List<TeamMate>
        {
            Hero1, Hero2, Hero3
        };
        
        readonly IEnumerable<PatronDay> _patronDays = new List<PatronDay>
        {
            new PatronDay(new LocalDate(2019, 11, 1), "LA")
        };

        readonly IEnumerable<Shift> _calendar = new List<Shift>
        {
            new Shift(Hero1, new LocalDate(2019, 12, 1)),
            new Shift(Hero2, new LocalDate(2019, 12, 2)),
            new Shift(Hero3, new LocalDate(2019, 12, 3)),
            new Shift(Hero1, new LocalDate(2019, 12, 4)),
            new Shift(Hero2, new LocalDate(2019, 12, 5))
        };

        [Test]
        public async Task Should_build_from_old_calendar()
        {
            _alertOwnerService
                .Setup(s => s.GetTeamMates())
                .ReturnsAsync(_teamMates);

            _alertOwnerService
                .Setup(s => s.GetCalendar(It.IsAny<IEnumerable<TeamMate>>()))
                .ReturnsAsync(_calendar);
            
            _alertOwnerService
                .Setup(s => s.GetPatronDays())
                .ReturnsAsync(_patronDays);

            _alertOwnerService
                .Setup(s => s.ClearCalendar())
                .Returns(Task.CompletedTask);

            _alertOwnerService
                .Setup(s => s.WriteCalendar(It.IsAny<List<Shift>>()))
                .Returns(Task.CompletedTask);

            _shiftsService
                .Setup(s => s.AddPatronDays(It.IsAny<IEnumerable<PatronDay>>()))
                .Returns(_shiftsService.Object);

            _shiftsService
                .Setup(s => s.Build(It.IsAny<IEnumerable<Shift>>()))
                .Returns(_calendar);

            _slackHttpClient
                .Setup(s => s.Notify(It.IsAny<string>()))
                .Returns(Task.CompletedTask);   
            
            _slackHttpClient
                .Setup(s => s.Notify(It.IsAny<IEnumerable<string>>()))
                .Returns(Task.CompletedTask);

            var sut = new CalendarJob(_alertOwnerService.Object, _slackHttpClient.Object, _shiftsService.Object,
                _converter, _logger);

            await sut.Execute(null);

            _shiftsService.Verify(s => s.Build(It.IsAny<IEnumerable<TeamMate>>()), Times.Never);
        }
        
        [Test]
        public async Task Should_build_new_calendar()
        {
            _alertOwnerService
                .Setup(s => s.GetTeamMates())
                .ReturnsAsync(_teamMates);

            _alertOwnerService
                .Setup(s => s.GetCalendar(It.IsAny<IEnumerable<TeamMate>>()))
                .ReturnsAsync(new List<Shift>());
            
            _alertOwnerService
                .Setup(s => s.GetPatronDays())
                .ReturnsAsync(_patronDays);

            _alertOwnerService
                .Setup(s => s.ClearCalendar())
                .Returns(Task.CompletedTask);

            _alertOwnerService
                .Setup(s => s.WriteCalendar(It.IsAny<List<Shift>>()))
                .Returns(Task.CompletedTask);

            _shiftsService
                .Setup(s => s.AddPatronDays(It.IsAny<IEnumerable<PatronDay>>()))
                .Returns(_shiftsService.Object);

            _shiftsService
                .Setup(s => s.Build(It.IsAny<IEnumerable<TeamMate>>()))
                .Returns(_calendar);

            _slackHttpClient
                .Setup(s => s.Notify(It.IsAny<string>()))
                .Returns(Task.CompletedTask);   
            
            _slackHttpClient
                .Setup(s => s.Notify(It.IsAny<IEnumerable<string>>()))
                .Returns(Task.CompletedTask);

            var sut = new CalendarJob(_alertOwnerService.Object, _slackHttpClient.Object, _shiftsService.Object,
                _converter, _logger);

            await sut.Execute(null);

            _shiftsService.Verify(s => s.Build(It.IsAny<IEnumerable<Shift>>()), Times.Never);
        }
    }
}