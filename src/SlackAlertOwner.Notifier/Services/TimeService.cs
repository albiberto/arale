namespace SlackAlertOwner.Notifier.Services
{
    using Abstract;
    using NodaTime;
    using System;

    public class TimeService : ITimeService
    {
        public LocalDate Now => LocalDate.FromDateTime(DateTime.Now);
    }
}