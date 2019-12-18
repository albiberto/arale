namespace SlackAlertOwner.Notifier.Model
{
    using NodaTime;
    using System;

    public class PatronDay
    {
        public DateTime Day { get; set; }
        public string CountryCode { get; set; }
    }
}