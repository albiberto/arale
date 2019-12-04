namespace SlackAlertOwner.Notifier
{
    using System;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class SlackHttpClient : ISlackHttpClient
    {
        readonly IHttpClientFactory _httpClientFactory;

        public SlackHttpClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task Notify()
        {
            using var client = _httpClientFactory.CreateClient("cazzeggingZoneClient");

            var payload = new
            {
                text = $"@Alucard is my hero! {DateTime.Now}"
            };

            var content = new StringContent(JsonSerializer.Serialize(payload));

            await client.PostAsync("m2eVtWuu7p7Ncf5ia5pCLSbr", content);
        }
    }
}