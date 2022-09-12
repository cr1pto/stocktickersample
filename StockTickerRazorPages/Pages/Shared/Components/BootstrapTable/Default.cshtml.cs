using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockTicker.Lib.Common.Models;

namespace StockTickerRazorPages.Pages.Shared.Component.BootstrapTable
{
    public class BootstrapTable : ViewComponent
    {
        [BindProperty]
        public QuoteResponse? QuoteResponse { get; set; }
        public async Task<IViewComponentResult> InvokeAsync(QuoteResponse? quoteResponse)
        {
            QuoteResponse = quoteResponse;
            return await Task.FromResult(View(quoteResponse));
        }

        public IViewComponentResult SortData(string fieldName)
        {
            var data = QuoteResponse?.Result?.OrderBy(t => fieldName).ToArray();
            QuoteResponse.Result = data;
            return View(QuoteResponse);
        }
    }
}
