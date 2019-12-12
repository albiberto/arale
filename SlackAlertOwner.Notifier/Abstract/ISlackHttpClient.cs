namespace SlackAlertOwner.Notifier.Abstract
{
    using System.Threading.Tasks;

    public interface ISlackHttpClient
    {
        Task Notify(object payload);
    }
}
