namespace SlackAlertOwner.Notifier.Clients
{
    using Abstract;
    using Microsoft.Extensions.Options;
    using Model;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class SlackHttpClient : ISlackHttpClient
    {
        readonly string _endpoint;
        readonly IHttpClientFactory _httpClientFactory;

        public SlackHttpClient(IHttpClientFactory httpClientFactory, IOptions<MyOptions> options)
        {
            _httpClientFactory = httpClientFactory;
            _endpoint = options.Value.EndPoint;
        }

        public async Task Notify(object payload)
        {
            using var client = _httpClientFactory.CreateClient("cazzeggingZoneClient");

            await client.PostAsync(_endpoint, new StringContent(JsonSerializer.Serialize(payload)));
        }
    }
}