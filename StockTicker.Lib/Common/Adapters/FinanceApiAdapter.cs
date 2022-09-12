using Microsoft.Extensions.Options;
using StockTicker.Lib.Common.Interfaces;
using StockTicker.Lib.Common.Models;
using System.Net.Http.Json;

namespace StockTicker.Lib.Common.Adapters
{
    public class FinanceApiAdapter : IFinanceApiAdapter
    {
        private const string region = "US";
        private const string lang = "en";
        private readonly HttpClient _client;
        private readonly IOptions<StockTickerApiConfiguration> _configuration;
        private readonly string apiRoot = "https://yfapi.net/v6";
        private readonly string financeQuoteRoute = "/finance/quote";

        public FinanceApiAdapter(HttpClient client, IOptions<StockTickerApiConfiguration> configuration)
        {
            _client = client;
            _configuration = configuration;
            _client.DefaultRequestHeaders.TryAddWithoutValidation("X-API-KEY", _configuration.Value.FinanceApiKey);
        }

        public async Task<YahooFinanceResponse?> GetStockInfoByTickers(string tickerSearchText, CancellationToken cancellationToken)
        {
            var tickerUri = BuildTickerQueryUrl(tickerSearchText);

            var response = await _client.GetAsync(tickerUri, cancellationToken);

            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            return await response.Content.ReadFromJsonAsync<YahooFinanceResponse>(cancellationToken: cancellationToken);
        }

        private Uri BuildTickerQueryUrl(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText)) throw new ArgumentNullException(nameof(searchText));

            return new UriBuilder(new Uri($"{apiRoot}{financeQuoteRoute}?region={region}&lang={lang}&symbols={searchText}")).Uri;
        }

    }
}
