using System.ComponentModel.DataAnnotations;

namespace api.DTOs.Stock;

public class StockCreateDto {
  [Required] [Length(4, 280)] public string CompanyName { get; set; } = "";
  [Required] [Length(4, 280)] public string Symbol { get; set; } = "";
  [Required] public decimal Purchase { get; set; }
  [Required] public decimal LastDividend { get; set; }
  [Required] [Length(4, 280)] public string Industry { get; set; } = "";
  [Required] public long MarketCap { get; set; }
}