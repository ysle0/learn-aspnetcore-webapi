using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

public class Stock {
  public int Id { get; init; }
  public string CompanyName { get; init; }
  public string Symbol { get; init; }

  [Column(TypeName = "decimal(18, 2)")] public decimal Purchase { get; init; }

  [Column(TypeName = "decimal(18, 2)")] public decimal LastDividend { get; init; }

  public string Industry { get; init; }
  public long MarketCap { get; init; }
  public List<Comment> Comments { get; init; }
}