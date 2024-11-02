namespace api.Controllers;

public record struct StockUpdateDto(
  string CompanyName,
  string Symbol,
  string Comments,
  decimal Purchase,
  decimal LastDividend,
  string Industry,
  long MarketCap
);