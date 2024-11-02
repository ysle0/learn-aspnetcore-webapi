namespace api.DTOs.Stock;

public record struct CreateStockRequestDto(
  string CompanyName,
  string Symbol,
  decimal Purchase,
  decimal LastDividend,
  string Industry,
  long MarketCap
);