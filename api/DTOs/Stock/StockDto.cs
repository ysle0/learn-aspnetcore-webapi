using api.DTOs.Comment;

namespace api.DTOs.Stock;

public record struct StockDto(
  int Id,
  string CompanyName,
  string Symbol,
  decimal Purchase,
  decimal LastDividend,
  string Industry,
  long MarketCap,
  List<CommentDto> Comments
);