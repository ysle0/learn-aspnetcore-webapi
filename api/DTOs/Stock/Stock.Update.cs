using System.ComponentModel.DataAnnotations;

namespace api.Controllers;

public class StockUpdateDto {
  [Required] [Length(4, 150)] public string CompanyName { get; set; }
  [Required] [Length(4, 4)] public string Symbol { get; set; }
  [Required] public decimal Purchase { get; set; }
  [Required] public decimal LastDividend { get; set; }
  [Required] [Length(4, 150)] public string Industry { get; set; }
  [Required] public long MarketCap { get; set; }
}