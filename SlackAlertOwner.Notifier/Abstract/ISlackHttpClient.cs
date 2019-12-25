namespace SlackAlertOwner.Notifier.Abstract
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ISlackHttpClient
    {
        Task Notify(string payload);
        Task Notify(IEnumerable<string> payload);
    }
}