using Microsoft.Extensions.Options;
using StockTicker.Lib.Common.Interfaces;
using StockTicker.Lib.Common.Models;
using System.Text.Json;
using System.Web;

namespace StockTicker.Lib.Common.Adapters;

public class StockTickerApiClient : IStockTickerApiClient
{
    private readonly HttpClient _client;
    private readonly IOptions<StockTickerApiConfiguration> _configuration;

    private Uri BuildTickerUri(string searchText) => new UriBuilder(new Uri($"{_configuration.Value.ApiRootAddress}/api/tickers/{HttpUtility.UrlEncode(searchText)}")).Uri;

    private Uri BuildHealthUri() => new UriBuilder(new Uri($"{_configuration.Value.ApiRootAddress}/health")).Uri;

    public StockTickerApiClient(HttpClient client, IOptions<StockTickerApiConfiguration> configuration )
    {
        _client = client;
        _configuration = configuration;
    }

    public async Task<YahooFinanceResponse?> GetStockInfoByTickers(string tickerSearchText, CancellationToken cancellationToken)
    {
        var uri = BuildTickerUri(tickerSearchText);
        var response = await _client.GetAsync(uri, cancellationToken);

        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(json)) return new EmptyQuoteTickerMetadata();

        var data = JsonSerializer.Deserialize<YahooFinanceResponse>(json);

        var results = data?.QuoteResponse?.Result?.Where(r => r.DisplayName != null);

        if (results != null && results.Any() && data?.QuoteResponse != null)
        {
            data.QuoteResponse.Result = results.ToArray();
        }

        return data;
    }

    public async Task<bool> GetApiHealth(CancellationToken cancellationToken)
    {
        var uri = BuildHealthUri();
        var response = await _client.GetAsync(uri, cancellationToken);

        response.EnsureSuccessStatusCode();
        var data = await response.Content.ReadAsStringAsync();
        return data != null;
    }
}
