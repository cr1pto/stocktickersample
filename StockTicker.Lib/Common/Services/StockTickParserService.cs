using StockTicker.Lib.Common.Interfaces;

namespace StockTicker.Lib.Common.Services;

public class StockTickParserService : IStockTickParserService
{
    public string ConvertStockTickersToCsvs(string? tickerSearchText)
    {
        if (tickerSearchText == null) return "";

        var tickers = tickerSearchText.Split(" ");
        return string.Join(",", tickers).ToUpper();
    }
}
