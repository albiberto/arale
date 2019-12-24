namespace SlackAlertOwner.Notifier.Abstract
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ISlackHttpClient
    {
        Task Notify(object payload);
        Task Notify(IEnumerable<object> payload);
    }
}
