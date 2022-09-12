namespace StockTicker.Lib.Common.Interfaces
{
    public interface IStockTickerApiClient : IFinanceApiAdapter
    {
        Task<bool> GetApiHealth(CancellationToken cancellationToken);
    }
}