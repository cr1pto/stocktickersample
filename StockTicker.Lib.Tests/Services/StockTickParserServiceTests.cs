using FluentAssertions;
using StockTicker.Lib.Common.Interfaces;
using StockTicker.Lib.Common.Services;

namespace StockTicker.Lib.Tests.Services;

public class StockTickParserServiceTests
{
    private IStockTickParserService _parserService;

    public StockTickParserServiceTests()
    {
        _parserService = new StockTickParserService();
    }

    [Fact]
    public void HappyPath()
    {
        var result = _parserService.ConvertStockTickersToCsvs("a b c");
        result.Should().BeEquivalentTo("a,b,c");
    }

    [Fact]
    public void GivenNoSpacesReturned_ShouldReturnDataSuccessfully()
    {
        var result = _parserService.ConvertStockTickersToCsvs("abc");
        result.Should().BeEquivalentTo("abc");
    }

    [Fact]
    public void GivenNullParameter_ShouldReturnDataSuccessfully()
    {
        var result = _parserService.ConvertStockTickersToCsvs(null);
        result.Should().BeEquivalentTo("");
    }
}