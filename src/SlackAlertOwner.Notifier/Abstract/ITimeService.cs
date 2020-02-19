namespace SlackAlertOwner.Notifier.Abstract
{
    using NodaTime;

    public interface ITimeService
    {
        LocalDate Now { get; }

        LocalDate NextMonth { get; }
    }
}