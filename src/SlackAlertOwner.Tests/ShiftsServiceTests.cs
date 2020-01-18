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

        readonly IEnumerable<TeamMate> _heroMates = new List<TeamMate>
        {
            new TeamMate("1", "IronMan", "US"),
            new TeamMate("2", "Hulk", "LA"),
            new TeamMate("3", "Thor", "AZ"),
            new TeamMate("4", "AntMan", "US")
        };

        static void CheckCompleteCalendar(IReadOnlyCollection<Shift> actual)
        {
            const int expectedCount = 9;
            Assert.AreEqual(expectedCount, actual.Count());

            const string expectedHero1 = "1";
            const string expectedCountryCode1 = "US";
            const string expectedHero2 = "2";
            const string expectedCountryCode2 = "LA";
            const string expectedHero3 = "3";
            const string expectedCountryCode3 = "AZ";
            const string expectedHero4 = "4";
            const string expectedCountryCode4 = "US";

            Assert.AreEqual(expectedHero1, actual.First().TeamMate.Id);
            Assert.AreEqual(expectedHero2, actual.Skip(1).First().TeamMate.Id);
            Assert.AreEqual(expectedHero3, actual.Skip(2).First().TeamMate.Id);
            Assert.AreEqual(expectedHero4, actual.Skip(3).First().TeamMate.Id);
            Assert.AreEqual(expectedHero1, actual.Skip(4).First().TeamMate.Id);
            Assert.AreEqual(expectedHero2, actual.Skip(5).First().TeamMate.Id);
            Assert.AreEqual(expectedHero3, actual.Skip(6).First().TeamMate.Id);
            Assert.AreEqual(expectedHero4, actual.Skip(7).First().TeamMate.Id);
            Assert.AreEqual(expectedHero1, actual.Skip(8).First().TeamMate.Id);
            Assert.AreEqual(expectedCountryCode1, actual.First().TeamMate.CountryCode);
            Assert.AreEqual(expectedCountryCode2, actual.Skip(1).First().TeamMate.CountryCode);
            Assert.AreEqual(expectedCountryCode3, actual.Skip(2).First().TeamMate.CountryCode);
            Assert.AreEqual(expectedCountryCode4, actual.Skip(3).First().TeamMate.CountryCode);
            Assert.AreEqual(expectedCountryCode1, actual.Skip(4).First().TeamMate.CountryCode);
            Assert.AreEqual(expectedCountryCode2, actual.Skip(5).First().TeamMate.CountryCode);
            Assert.AreEqual(expectedCountryCode3, actual.Skip(6).First().TeamMate.CountryCode);
            Assert.AreEqual(expectedCountryCode4, actual.Skip(7).First().TeamMate.CountryCode);
            Assert.AreEqual(expectedCountryCode1, actual.Skip(8).First().TeamMate.CountryCode);
        }


        [Test]
        public void Infinity()
        {
            var sut = new ShiftsService(() => _calendarService.Build());

            var actual = sut
                .AddPatronDay(new PatronDay(new LocalDate(2019, 12, 6), "US"))
                .AddPatronDay(new PatronDay(new LocalDate(2019, 12, 6), "LA"))
                .AddPatronDay(new PatronDay(new LocalDate(2019, 12, 6), "AZ"))
                .Build(_heroMates)
                .ToList();
        }

        [Test]
        public void Should_return_shifts_calendar_with_one_patron_day_but_non_in_current_mount()
        {
            var patronDay = new PatronDay(new LocalDate(2019, 10, 25), "LA");
            
            var sut = new ShiftsService(() => _calendarService.Build());

            var actual = sut
                .AddPatronDay(patronDay)
                .Build(_heroMates)
                .ToList();

            CheckCompleteCalendar(actual);
        }

        [Test]
        public void Should_return_shifts_calendar_with_one_patron_day_check()
        {
            var patronDay = new PatronDay(new LocalDate(2019, 12, 5), "US");
            
            var sut = new ShiftsService(() => _calendarService.Build());

            var actual = sut
                .AddPatronDay(patronDay)
                .Build(_heroMates)
                .ToList();

            const string expectedCandidateHero = "2";
            const string expectedCandidateCountryCode = "LA";
            var expectedCandidateDay = patronDay.Day;

            Assert.AreEqual(expectedCandidateHero, actual.First(a => a.Schedule == expectedCandidateDay).TeamMate.Id);
            Assert.AreEqual(expectedCandidateCountryCode,
                actual.First(a => a.Schedule == expectedCandidateDay).TeamMate.CountryCode);

            const string expectedHero = "1";
            var expectedCountryCode = patronDay.CountryCode;
            var expectedDay = patronDay.Day.PlusDays(1);

            Assert.AreEqual(expectedHero, actual.First(a => a.Schedule == expectedDay).TeamMate.Id);
            Assert.AreEqual(expectedCountryCode, actual.First(a => a.Schedule == expectedDay).TeamMate.CountryCode);
        }

        [Test]
        public void Should_return_shifts_calendar_with_two_patron_day_check_and_direct_switch()
        {
            var patronDay1 = new PatronDay(new LocalDate(2019, 12, 5), "US");
            var patronDay2 = new PatronDay(new LocalDate(2019, 12, 6), "LA");

            var sut = new ShiftsService(() => _calendarService.Build());

            var actual = sut
                .AddPatronDay(patronDay1)
                .AddPatronDay(patronDay2)
                .Build(_heroMates)
                .ToList();

            const string expectedCandidateHero = "2";
            const string expectedCandidateCountryCode = "LA";
            var expectedCandidateDay = patronDay1.Day;

            Assert.AreEqual(expectedCandidateHero, actual.First(a => a.Schedule == expectedCandidateDay).TeamMate.Id);
            Assert.AreEqual(expectedCandidateCountryCode,
                actual.First(a => a.Schedule == expectedCandidateDay).TeamMate.CountryCode);

            const string expectedHero = "1";
            var expectedCountryCode = patronDay1.CountryCode;
            var expectedDay = patronDay1.Day.PlusDays(1);

            Assert.AreEqual(expectedHero, actual.First(a => a.Schedule == expectedDay).TeamMate.Id);
            Assert.AreEqual(expectedCountryCode, actual.First(a => a.Schedule == expectedDay).TeamMate.CountryCode);
        }

        [Test]
        public void Should_return_shifts_calendar_without_patron_days_check()
        {
            var sut = new ShiftsService(() => _calendarService.Build());

            var actual = sut
                .Build(_heroMates)
                .ToList();

            CheckCompleteCalendar(actual);
        }

        [Test]
        public void Should_use_old_calendar()
        {
            var sut = new ShiftsService(() => _calendarService.Build());

            var calendarOld = new List<Shift>
            {
                new Shift(new TeamMate("1", "IronMan", "US"), new LocalDate(2019, 11, 1)),
                new Shift(new TeamMate("3", "Thor", "AZ"), new LocalDate(2019, 11, 2)),
                new Shift(new TeamMate("2", "Hulk", "LA"), new LocalDate(2019, 11, 3)),
                new Shift(new TeamMate("4", "AntMan", "US"), new LocalDate(2019, 11, 4)),
                new Shift(new TeamMate("1", "IronMan", "US"), new LocalDate(2019, 11, 5)),
                new Shift(new TeamMate("3", "Thor", "AZ"), new LocalDate(2019, 11, 6)),
                new Shift(new TeamMate("2", "Hulk", "LA"), new LocalDate(2019, 11, 7)),
                new Shift(new TeamMate("4", "AntMan", "US"), new LocalDate(2019, 11, 8)),
                new Shift(new TeamMate("1", "IronMan", "US"), new LocalDate(2019, 11, 9)),
                new Shift(new TeamMate("3", "Thor", "AZ"), new LocalDate(2019, 11, 10))
            };

            var actual = sut
                .Build(calendarOld)
                .ToList();

            var expected = new List<Shift>
            {
                new Shift(new TeamMate("2", "Hulk", "LA"), new LocalDate(2019, 12, 1)),
                new Shift(new TeamMate("4", "AntMan", "US"), new LocalDate(2019, 12, 2)),
                new Shift(new TeamMate("1", "IronMan", "US"), new LocalDate(2019, 12, 3)),
                new Shift(new TeamMate("3", "Thor", "AZ"), new LocalDate(2019, 12, 4)),
                new Shift(new TeamMate("2", "Hulk", "LA"), new LocalDate(2019, 12, 5)),
                new Shift(new TeamMate("4", "AntMan", "US"), new LocalDate(2019, 12, 6)),
                new Shift(new TeamMate("1", "IronMan", "US"), new LocalDate(2019, 12, 7)),
                new Shift(new TeamMate("3", "Thor", "AZ"), new LocalDate(2019, 12, 8)),
                new Shift(new TeamMate("2", "Hulk", "LA"), new LocalDate(2019, 12, 9))
            };

            for (var i = 0; i < actual.Count; i++)
            {
                var e = expected.Skip(i).First();
                var a = actual.Skip(i).First();

                Assert.AreEqual(e.Schedule, a.Schedule);
                Assert.AreEqual(e.TeamMate.Id, a.TeamMate.Id);
            }
        }
    }
}