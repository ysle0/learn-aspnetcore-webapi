using System.ComponentModel.DataAnnotations;

namespace api.DTOs.Comment;

public class CommentDto {
  public int Id { get; set; }
  [Required] [Length(5, 280)] public string Title { get; set; } = "";
  [Required] [Length(5, 280)] public string Content { get; set; } = "";
  public int? StockId { get; set; }
  public DateTime CreatedOn { get; set; } = DateTime.Now;
}