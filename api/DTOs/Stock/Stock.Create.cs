namespace api.DTOs.Stock;

public record struct StockCreateDto(
  string CompanyName,
  string Symbol,
  decimal Purchase,
  decimal LastDividend,
  string Industry,
  long MarketCap
);