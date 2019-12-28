namespace SlackAlertOwner.Tests
{
    using Moq;
    using NodaTime;
    using Notifier.Abstract;
    using Notifier.Model;
    using Notifier.Services;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class ShiftsServiceTests
    {
        [SetUp]
        public void Setup()
        {
            _repository = new MockRepository(MockBehavior.Strict) {DefaultValue = DefaultValue.Mock};

            _timeService = _repository.Create<ITimeService>();
            _randomService = _repository.Create<IRandomIndexService>();

            var calendarService = _repository.Create<ICalendarService>();

            calendarService
                .Setup(s => s.Build())
                .Returns(new List<LocalDate>
                {
                    new LocalDate(2019, 12, 1),
                    new LocalDate(2019, 12, 2),
                    new LocalDate(2019, 12, 3),
                    new LocalDate(2019, 12, 4),
                    new LocalDate(2019, 12, 5),
                    new LocalDate(2019, 12, 6),
                    new LocalDate(2019, 12, 7),
                    new LocalDate(2019, 12, 8),
                    new LocalDate(2019, 12, 9)
                });

            _calendarService = calendarService.Object;
        }

        [TearDown]
        public void VerifyAndTearDown()
        {
            _repository.VerifyAll();
        }

        MockRepository _repository;
        ICalendarService _calendarService;
        Mock<ITimeService> _timeService;
        Mock<IRandomIndexService> _randomService;

        readonly IEnumerable<TeamMate> _heroMates = new List<TeamMate>
        {
            new TeamMate("1", "IronMan", "US"),
            new TeamMate("2", "Hulk", "LA"),
            new TeamMate("3", "Thor", "AZ"),
            new TeamMate("4", "AntMan", "US")
        };

        [Test]
        public void Should_return_calendar_with_one_patron_day_but_non_in_current_mount()
        {
            var sut = new ShiftsService(() => _calendarService.Build(), _timeService.Object, _randomService.Object);

            var actual = sut
                .Build(_heroMates)
                .ToList();

            const int expectedCount = 9;
            Assert.AreEqual(expectedCount, actual.Count());

            const string expectedHero = "1";
            const string expectedCountryCode = "US";

            Assert.AreEqual(expectedHero, actual.First().TeamMate.Id);
            Assert.AreEqual(expectedHero, actual.Skip(4).First().TeamMate.Id);
            Assert.AreEqual(expectedHero, actual.Skip(8).First().TeamMate.Id);
            Assert.AreEqual(expectedCountryCode, actual.First().TeamMate.CountryCode);
            Assert.AreEqual(expectedCountryCode, actual.Skip(4).First().TeamMate.CountryCode);
            Assert.AreEqual(expectedCountryCode, actual.Skip(8).First().TeamMate.CountryCode);
        }

        [Test]
        public void Should_return_shifts_calendar_without_patron_days_check()
        {
            _randomService.Setup(s => s
                .Random(It.Is<int>(last => last == 9)))
                .Returns(2);
            
            var sut = new ShiftsService(() => _calendarService.Build(), _timeService.Object, _randomService.Object);

            var patronDay = new LocalDate(2019, 12, 5);
            
            var actual = sut
                .AddPatronDay(new PatronDay(patronDay, "LA"))
                .Build(_heroMates)
                .ToList();

            const string expectedHero = "2";
            const string expectedCountryCode = "LA";

            Assert.AreEqual(expectedHero, actual.First(a => a.Schedule == patronDay ).TeamMate.Id);
            Assert.AreEqual(expectedHero, actual.First(a => a.Schedule == patronDay ).TeamMate.CountryCode);
        }
    }
}