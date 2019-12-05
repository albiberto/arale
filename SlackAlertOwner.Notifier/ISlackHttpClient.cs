namespace SlackAlertOwner.Notifier
{
    using System.Threading.Tasks;

    public interface ISlackHttpClient
    {
        Task Notify(object payload);
    }
}
