namespace api.Controllers;

public record struct UpdateCommentDto(
  string Title,
  string Content,
  int? StockId
);