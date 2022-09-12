using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using StockTicker.Lib.Common.Adapters;
using StockTicker.Lib.Common.Interfaces;
using StockTicker.Lib.Common.Models;
using System.Text.Json;

namespace StockTicker.Lib.Tests.Adapters;

public class FinanceApiAdapterTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly IFinanceApiAdapter _financeApiAdapter;
    private static HttpClient? _httpClient;
    private readonly Mock<IOptions<StockTickerApiConfiguration>> _mockConfig;

    public FinanceApiAdapterTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _mockConfig = new Mock<IOptions<StockTickerApiConfiguration>>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        GivenDefaultConfigOptions();
        _financeApiAdapter = new FinanceApiAdapter(_httpClient, _mockConfig.Object);

    }

    [Fact]
    public async Task GivenStockTickerInfoIsCalled_ReturnsSuccess()
    {
        var fakeContent = GivenFakeObjectJson();
        GivenSuccessfulGetAsync(fakeContent);
        var result = await _financeApiAdapter.GetStockInfoByTickers("abc", CancellationToken.None);

        result.Should().NotBeNull();
        result?.QuoteResponse?.Result[0]?.DisplayName.Should().BeEquivalentTo("fake");
    }

    [Fact]
    public async Task GivenStockInfoIsCalled_ThrowsException()
    {
        var fakeContent = GivenFakeObjectJson();
        GivenFailureGetAsync(fakeContent);
        await Assert.ThrowsAsync<HttpRequestException>(() => _financeApiAdapter.GetStockInfoByTickers("abc", CancellationToken.None));
    }

    private void GivenSuccessfulGetAsync(string fakeContent)
    {
        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
         .ReturnsAsync(new HttpResponseMessage
         {
             StatusCode = System.Net.HttpStatusCode.OK,
             Content = new StringContent(fakeContent)
         });
    }

    private void GivenFailureGetAsync(string fakeContent)
    {
        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException());
    }

    private void GivenDefaultConfigOptions()
    {
        _mockConfig.Setup(config => config.Value).Returns(() => new StockTickerApiConfiguration
        {
            ApiRootAddress = "http://fake.com",
            FinanceApiKey = "12341234fake"
        });
    }

    private string GivenFakeObjectJson()
    {
        return JsonSerializer.Serialize(new YahooFinanceResponse
        {
            QuoteResponse = new QuoteResponse
            {
                Result = new Result[1]
                {
                    new Result
                    {
                        DisplayName = "fake"
                    }
                }
            }
        });
    }
}
