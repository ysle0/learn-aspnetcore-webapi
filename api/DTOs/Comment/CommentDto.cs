using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace api.DTOs.Comment;

public class CommentDto {
  public int Id { get; set; }

  [Required]
  [Length(5, 280)]
  [JsonPropertyName("title")]
  public string Title { get; set; } = "";

  [Required]
  [Length(5, 280)]
  [JsonPropertyName("content")]
  public string Content { get; set; } = "";

  public int? StockId { get; set; }

  [JsonPropertyName("createdOn")]
  public DateTime CreatedOn { get; set; } = DateTime.Now;
}