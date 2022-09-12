using Microsoft.AspNetCore.Mvc;
using StockTicker.Lib.Common.Interfaces;

namespace StockTickerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TickersController : ControllerBase
    {
        private readonly IFinanceApiAdapter _financeApiAdapter;

        public TickersController(IFinanceApiAdapter financeApiAdapter)
        {
            _financeApiAdapter = financeApiAdapter;
        }

        [Route("{tickerSearchText}")]
        [HttpGet]
        public async Task<ActionResult> Get(string tickerSearchText, CancellationToken cancellationToken)
        {
            var data = await _financeApiAdapter.GetStockInfoByTickers(tickerSearchText, cancellationToken);

            return Ok(data);
        }
    }
}
