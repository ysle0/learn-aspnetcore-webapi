using System.ComponentModel.DataAnnotations;

namespace api.DTOs.Comment;

public class CreateCommentDto {
  [Required] [Length(5, 280)] public string Title { get; set; } = "";
  [Required] [Length(5, 280)] public string Content { get; set; } = "";
}