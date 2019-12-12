namespace SlackAlertOwner.Notifier.Abstract
{
    using NodaTime;
    using System.Collections.Generic;

    public interface ILocalDateService
    {
        LocalDate Parse(IEnumerable<object> value);
    }
}