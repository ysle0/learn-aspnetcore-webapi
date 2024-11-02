namespace api.Controllers;

public record struct UpdateStockRequestDto(
  string CompanyName,
  string Symbol,
  string Comments,
  decimal Purchase,
  decimal LastDividend,
  string Industry,
  long MarketCap
);