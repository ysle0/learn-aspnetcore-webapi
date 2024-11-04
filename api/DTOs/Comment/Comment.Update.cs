using System.ComponentModel.DataAnnotations;

namespace api.Controllers;

public class UpdateCommentDto {
  [Required] [Length(5, 280)] public string Title { get; set; } = "";
  [Required] [Length(5, 280)] public string Content { get; set; } = "";
}