using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockTicker.Lib.Common.Interfaces;
using StockTicker.Lib.Common.Models;

namespace StockTickerRazorPages.Pages
{
    public class IndexModel : PageModel 
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IStockTickerApiClient _stockTickerApiClient;
        private readonly IStockTickParserService _stockTickParserService;

        [BindProperty]
        public YahooFinanceResponse? StockTickers { get; set; }

        [BindProperty]
        public string? SearchText { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IStockTickerApiClient stockTickerApiClient, IStockTickParserService stockTickParserService)
        {
            _logger = logger;
            _stockTickerApiClient = stockTickerApiClient;
            _stockTickParserService = stockTickParserService;
        }

        public async Task<IActionResult> OnGet(CancellationToken cancellationToken)
        {
            try
            {
                var isApiHealthy = await _stockTickerApiClient.GetApiHealth(cancellationToken);
            }
            catch(Exception ex)
            {
                _logger.LogCritical(ex.ToString());
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }

            if(SearchText == null || string.IsNullOrWhiteSpace(SearchText))
            {
                return Page();
            }
            var stockTickers = _stockTickParserService.ConvertStockTickersToCsvs(SearchText);

            StockTickers = await _stockTickerApiClient.GetStockInfoByTickers(stockTickers, cancellationToken);
            
            return Page();
        }
    }
}