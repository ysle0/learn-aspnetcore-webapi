namespace api.DTOs.Comment;

public record struct CommentDto(
  int Id,
  string Title,
  string Content,
  int? StockId,
  DateTime CreatedOn
);