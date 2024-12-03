namespace DaemonNET.Client
{
    using System.Globalization;
    using System.Web;
    using DaemonNET.Configuration;

    public class DaemonHttpClient
    {
        public DaemonHttpClient(IDaemonConfiguration daemonConfiguration, HttpClient httpClient)
        {
            this._httpClient = httpClient;
            this._daemonConfiguration = daemonConfiguration;
        }

        private readonly HttpClient _httpClient;

        private readonly IDaemonConfiguration _daemonConfiguration;

        public async Task SendAsync(DateTimeOffset execution, CancellationToken cancellationToken = default)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["execution"] = execution.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);

            UriBuilder urlBuilder = new UriBuilder(this._daemonConfiguration.RouteCallback.Host)
            {
                Query = string.Join("&", query.ToString())
            };

            using (var request = new HttpRequestMessage(HttpMethod.Get, urlBuilder.Uri.AbsoluteUri))
            {
                var response = await this._httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }
        }
    }
}
