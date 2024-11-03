namespace api.DTOs.Comment;

public record struct CreateCommentDto(
  string Title,
  string Content
);