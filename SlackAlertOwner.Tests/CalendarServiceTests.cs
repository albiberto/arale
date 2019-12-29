namespace SlackAlertOwner.Tests
{
    using Moq;
    using NodaTime;
    using Notifier.Abstract;
    using Notifier.Services;
    using NUnit.Framework;
    using System.Linq;

    [TestFixture]
    public class CalendarServiceTests
    {
        [SetUp]
        public void Setup()
        {
            var timeService = new Mock<ITimeService>();
            timeService
                .Setup(s => s.Now)
                .Returns(new LocalDate(2019, 12, 25));

            _timeService = timeService.Object;
        }

        ITimeService _timeService;

        [Test]
        public void Should_return_calendar_without_weekends()
        {
            var sut = new CalendarService(_timeService);

            var calendar = sut
                .WithoutWeekEnd()
                .Build()
                .ToList();

            const int expectedCount = 22;
            Assert.AreEqual(expectedCount, calendar.Count());

            const string expected =
                "Monday, December 2, 2019;Tuesday, December 3, 2019;Wednesday, December 4, 2019;Thursday, December 5, 2019;Friday, December 6, 2019;Monday, December 9, 2019;Tuesday, December 10, 2019;Wednesday, December 11, 2019;Thursday, December 12, 2019;Friday, December 13, 2019;Monday, December 16, 2019;Tuesday, December 17, 2019;Wednesday, December 18, 2019;Thursday, December 19, 2019;Friday, December 20, 2019;Monday, December 23, 2019;Tuesday, December 24, 2019;Wednesday, December 25, 2019;Thursday, December 26, 2019;Friday, December 27, 2019;Monday, December 30, 2019;Tuesday, December 31, 2019";
            var actual = string.Join(";", calendar.Select(day => day.ToString()));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Should_return_calendar_without_holidays()
        {
            var sut = new CalendarService(_timeService);

            var calendar = sut
                .WithoutHolidays()
                .Build()
                .ToList();

            const int expectedCount = 28;
            Assert.AreEqual(expectedCount, calendar.Count());

            const string expected =
                "Sunday, December 1, 2019;Monday, December 2, 2019;Tuesday, December 3, 2019;Wednesday, December 4, 2019;Thursday, December 5, 2019;Friday, December 6, 2019;Saturday, December 7, 2019;Monday, December 9, 2019;Tuesday, December 10, 2019;Wednesday, December 11, 2019;Thursday, December 12, 2019;Friday, December 13, 2019;Saturday, December 14, 2019;Sunday, December 15, 2019;Monday, December 16, 2019;Tuesday, December 17, 2019;Wednesday, December 18, 2019;Thursday, December 19, 2019;Friday, December 20, 2019;Saturday, December 21, 2019;Sunday, December 22, 2019;Monday, December 23, 2019;Tuesday, December 24, 2019;Friday, December 27, 2019;Saturday, December 28, 2019;Sunday, December 29, 2019;Monday, December 30, 2019;Tuesday, December 31, 2019";
            var actual = string.Join(";", calendar.Select(day => day.ToString()));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Should_return_calendar_with_only_working_days()
        {
            var sut = new CalendarService(_timeService);

            var calendar = sut
                .WithoutHolidays()
                .WithoutWeekEnd()
                .Build()
                .ToList();

            const int expectedCount = 20;
            Assert.AreEqual(expectedCount, calendar.Count());

            const string expected =
                "Monday, December 2, 2019;Tuesday, December 3, 2019;Wednesday, December 4, 2019;Thursday, December 5, 2019;Friday, December 6, 2019;Monday, December 9, 2019;Tuesday, December 10, 2019;Wednesday, December 11, 2019;Thursday, December 12, 2019;Friday, December 13, 2019;Monday, December 16, 2019;Tuesday, December 17, 2019;Wednesday, December 18, 2019;Thursday, December 19, 2019;Friday, December 20, 2019;Monday, December 23, 2019;Tuesday, December 24, 2019;Friday, December 27, 2019;Monday, December 30, 2019;Tuesday, December 31, 2019";
            var actual = string.Join(";", calendar.Select(day => day.ToString()));

            Assert.AreEqual(expected, actual);
        }
        
        [Test]
        public void Should_return_complete_calendar()
        {
            var sut = new CalendarService(_timeService);

            var calendar = sut
                .Build()
                .ToList();

            const int expectedCount = 31;
            Assert.AreEqual(expectedCount, calendar.Count());

            const string expected =
                "Sunday, December 1, 2019;Monday, December 2, 2019;Tuesday, December 3, 2019;Wednesday, December 4, 2019;Thursday, December 5, 2019;Friday, December 6, 2019;Saturday, December 7, 2019;Sunday, December 8, 2019;Monday, December 9, 2019;Tuesday, December 10, 2019;Wednesday, December 11, 2019;Thursday, December 12, 2019;Friday, December 13, 2019;Saturday, December 14, 2019;Sunday, December 15, 2019;Monday, December 16, 2019;Tuesday, December 17, 2019;Wednesday, December 18, 2019;Thursday, December 19, 2019;Friday, December 20, 2019;Saturday, December 21, 2019;Sunday, December 22, 2019;Monday, December 23, 2019;Tuesday, December 24, 2019;Wednesday, December 25, 2019;Thursday, December 26, 2019;Friday, December 27, 2019;Saturday, December 28, 2019;Sunday, December 29, 2019;Monday, December 30, 2019;Tuesday, December 31, 2019";
            var actual = string.Join(";", calendar.Select(day => day.ToString()));

            Assert.AreEqual(expected, actual);
        }
    }
}