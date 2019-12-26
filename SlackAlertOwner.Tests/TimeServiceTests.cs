using NUnit.Framework;

namespace SlackAlertOwner.Tests
{
    using NodaTime;
    using Notifier.Abstract;
    using Notifier.Services;
    using System;

    [TestFixture]
    public class TimeServiceTests
    {
        ITimeService _sut; 
        
        [SetUp]
        public void Setup()
        {
            _sut = new TimeService();
        }

        [Test]
        public void Should_return_correct_date()
        {
            var today = DateTime.Now;
            var expected = new LocalDate(today.Year, today.Month, today.Day);
            
            var actual = _sut.Now;
            
            Assert.AreEqual(expected, actual);
        }
    }
}