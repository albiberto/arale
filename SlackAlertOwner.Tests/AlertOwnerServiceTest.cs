#nullable enable
namespace SlackAlertOwner.Tests
{
    using Castle.Components.DictionaryAdapter;
    using Google.Apis.Sheets.v4.Data;
    using Microsoft.Extensions.Options;
    using Moq;
    using NodaTime;
    using Notifier.Abstract;
    using Notifier.Model;
    using Notifier.Services;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [TestFixture]
    public class AlertOwnerServiceTest
    {
        [SetUp]
        public void Setup()
        {
            _repository = new MockRepository(MockBehavior.Strict) {DefaultValue = DefaultValue.Mock};

            _googleSpreadSheetClient = _repository.Create<IGoogleSpreadSheetClient>();

            _converter = _repository.Create<ITypeConverter<LocalDate>>();

            _timeService = _repository.Create<ITimeService>();


            var options = _repository.Create<IOptions<MyOptions>>();

            options
                .Setup(s => s.Value)
                .Returns(new MyOptions
                {
                    SpreadsheetId = "1",
                    CalendarRange = "Calendar!A:B",
                    TeamMatesRange = "TeamMates!A:C",
                    PatronDaysRange = "PatronDays!A:B"
                });

            _options = options.Object;
        }

        [TearDown]
        public void VerifyAndTearDown()
        {
            _repository.VerifyAll();
        }

        MockRepository _repository;
        Mock<IGoogleSpreadSheetClient> _googleSpreadSheetClient;
        Mock<ITypeConverter<LocalDate>> _converter;
        IOptions<MyOptions> _options;
        Mock<ITimeService> _timeService;

        [Test]
        public async Task Should_return_patronDays()
        {
            _googleSpreadSheetClient
                .Setup(s =>
                    s.Get(It.Is<string>(id => id == "1"), It.Is<string>(range => range == "PatronDays!A:B")))
                .ReturnsAsync(new ValueRange
                {
                    Values = new List<IList<object>>
                    {
                        new List<object>
                        {
                            "25/12", "LP"
                        }
                    }
                });

            var sut = new AlertOwnerService(_googleSpreadSheetClient.Object, _converter.Object, _timeService.Object, _options);
            
            var actual = (await sut.GetPatronDays()).First();

            Assert.AreEqual(new LocalDate(2019, 12, 25).ToString(), actual.Day.ToString());
            Assert.AreEqual("LP", actual.CountryCode);
        }

        [Test]
        public async Task Should_return_shifts()
        {
            _googleSpreadSheetClient
                .Setup(s =>
                    s.Get(It.Is<string>(id => id == "1"), It.Is<string>(range => range == "Calendar!A:B")))
                .ReturnsAsync(new ValueRange
                {
                    Values = new List<IList<object>>
                    {
                        new List<object>
                        {
                            "25/12/2019", "IronMan"
                        },
                        new List<object>
                        {
                            "26/12/2019", "Hulk"
                        },
                        new List<object>
                        {
                            "27/12/2019", "Thor"
                        },
                        new List<object>
                        {
                            "28/12/2019", "IronMan"
                        }
                    }
                });

            var teamMates = new List<TeamMate>
            {
                new TeamMate("1", "IronMan", "LA"),
                new TeamMate("2", "Hulk", "NY"),
                new TeamMate("3", "Thor", "AZ")
            };

            _timeService
                .Setup(s => s.Now)
                .Returns(new LocalDate(2019, 12, 25));

            _converter.SetupSequence(s => s.ParseValueFromString(It.IsAny<string>()))
                .Returns(new LocalDate(2019, 12, 25))
                .Returns(new LocalDate(2019, 12, 26));

            var sut = new AlertOwnerService(_googleSpreadSheetClient.Object, _converter.Object, _timeService.Object, _options);
            
            var (today, tomorrow) = await sut.GetShift(teamMates);

            Assert.AreEqual(new LocalDate(2019, 12, 25).ToString(), today.Schedule.ToString());
            Assert.AreEqual("1", today.TeamMate.Id);
            Assert.AreEqual("IronMan", today.TeamMate.Name);
            Assert.AreEqual(null, today.TeamMate.CountryCode);

            Assert.AreEqual(new LocalDate(2019, 12, 26).ToString(), tomorrow.Schedule.ToString());
            Assert.AreEqual("2", tomorrow.TeamMate.Id);
            Assert.AreEqual("Hulk", tomorrow.TeamMate.Name);
            Assert.AreEqual(null, tomorrow.TeamMate.CountryCode);
        }

        [Test]
        public async Task Should_return_teamMates()
        {
            _googleSpreadSheetClient
                .Setup(s =>
                    s.Get(It.Is<string>(id => id == "1"), It.Is<string>(range => range == "TeamMates!A:C")))
                .ReturnsAsync(new ValueRange
                {
                    Values = new List<IList<object>>
                    {
                        new List<object>
                        {
                            "IronMan", "1", "LA"
                        }
                    }
                });

            var sut = new AlertOwnerService(_googleSpreadSheetClient.Object, _converter.Object, _timeService.Object, _options);
            
            var actual = (await sut.GetTeamMates()).ToList().First();

            Assert.AreEqual("1", actual.Id);
            Assert.AreEqual("IronMan", actual.Name);
            Assert.AreEqual("LA", actual.CountryCode);
        }

        [Test]
        public async Task Should_clear_calendar()
        {
            _googleSpreadSheetClient
                .Setup(s =>
                    s.Clear(
                        It.Is<string>(id => id == "1"), 
                        It.Is<string>(range => range == "Calendar!A:B"),
                        It.IsAny<ClearValuesRequest>())
                )
                .ReturnsAsync(new ClearValuesResponse
                {
                    SpreadsheetId = "1"
                });
            
            var sut = new AlertOwnerService(_googleSpreadSheetClient.Object, _converter.Object, _timeService.Object, _options);

            await sut.ClearCalendar();

            _googleSpreadSheetClient.Verify(s => s.Clear(
                It.Is<string>(id => id == "1"),
                It.Is<string>(range => range == "Calendar!A:B"),
                It.IsAny<ClearValuesRequest>()), Times.Once
            );
        }
    }
}