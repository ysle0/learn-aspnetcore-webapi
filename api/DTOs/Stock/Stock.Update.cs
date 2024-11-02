namespace api.Controllers;

public record struct StockUpdateDto(
  string? CompanyName,
  string? Symbol,
  decimal? Purchase,
  decimal? LastDividend,
  string? Industry,
  long? MarketCap
);