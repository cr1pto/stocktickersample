namespace StockTicker.Lib.Common.Interfaces;

public interface IStockTickParserService
{
    string ConvertStockTickersToCsvs(string? tickerSearchText);
}