namespace SlackAlertOwner.Notifier.Model
{
    using NodaTime;

    public class PatronDay
    {
        public LocalDate Day { get; set; }
        public string CountryCode { get; set; }
    }
}