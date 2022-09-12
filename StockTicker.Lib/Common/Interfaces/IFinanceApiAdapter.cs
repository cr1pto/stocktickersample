using StockTicker.Lib.Common.Models;

namespace StockTicker.Lib.Common.Interfaces;

public interface IFinanceApiAdapter
{
    Task<YahooFinanceResponse?> GetStockInfoByTickers(string tickerSearchText, CancellationToken cancellationToken);
}
