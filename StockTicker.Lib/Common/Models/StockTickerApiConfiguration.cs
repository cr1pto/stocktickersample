using System.ComponentModel.DataAnnotations;

namespace StockTicker.Lib.Common.Models;

public class StockTickerApiConfiguration
{
    [Required]
    public string? ApiRootAddress { get; set; }

    [Required]
    public string? FinanceApiKey { get; set; }
}