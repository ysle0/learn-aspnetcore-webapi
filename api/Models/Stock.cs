using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

public record Stock(
  int Id,
  string CompanyName,
  string Symbol,
  [property: Column(TypeName = "decimal(18, 2)")]
  decimal Purchase,
  [property: Column(TypeName = "decimal(18, 2)")]
  decimal LastDividend,
  string Industry,
  long MarketCap,
  List<Comment> Comments
);