namespace SlackAlertOwner.Notifier.Model
{
    using Abstract;
    using NodaTime;

    public class PatronDay : IDay
    {
        public PatronDay(LocalDate day, string countryCode)
        {
            Day = day;
            CountryCode = countryCode;
        }
        public LocalDate Day { get; }
        public string CountryCode { get; }
    }
}