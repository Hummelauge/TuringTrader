#if EXTENSION

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using TuringTrader.Simulator;


namespace TuringTrader.SimulatorV2
{
    public class ServiceTiingoTicker
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ServiceTiingoTicker(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetTiingoData(string symbolTiingo)
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(10);

            string url = string.Format(
                "https://api.tiingo.com/tiingo/daily/{0}/prices"
                + "&format=json"
                + "?startDate={1:yyyy}-{1:MM}-{1:dd}"
                + "&resampleFreq=daily",
                symbolTiingo,
                DateTime.Parse("01/01/1950", CultureInfo.InvariantCulture));

            using (HttpRequestMessage request = new(HttpMethod.Get, url))
            {
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Host = request.RequestUri.Host;
                request.Headers.Add("Connection", "Close");
                request.Headers.Date = new DateTimeOffset(DateTime.Now);
                request.Headers.Add("Accept-Language", "en-us");
                request.Headers.Add("Cache-Control", "no-cache");
                request.Headers.Authorization = new AuthenticationHeaderValue("Token", GlobalSettings.TiingoApiKey);

                HttpResponseMessage response = await httpClient.SendAsync(request).ConfigureAwait(false);
                string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return json;
            }
        }
    }
}
#endif