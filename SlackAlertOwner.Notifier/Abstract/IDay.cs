namespace SlackAlertOwner.Notifier.Abstract
{
    using NodaTime;

    public interface IDay
    {
        LocalDate Day { get; }
    }
}