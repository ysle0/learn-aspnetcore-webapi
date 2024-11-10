using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using api.DTOs.Comment;

namespace api.DTOs.Stock;

public struct StockDto {
  [Required]
  [Length(4, 280)]
  [JsonPropertyName("companyName")]
  public string CompanyName { get; set; }

  [Required]
  [Length(4, 280)]
  [JsonPropertyName("symbol")]
  public string Symbol { get; set; }

  [Required]
  [JsonPropertyName("purchase")]
  public decimal Purchase { get; set; }

  [Required]
  [JsonPropertyName("lastDividend")]
  public decimal LastDividend { get; set; }

  [Required]
  [Length(4, 280)]
  [JsonPropertyName("industry")]
  public string Industry { get; set; }

  [Required]
  [JsonPropertyName("marketCap")]
  public long MarketCap { get; set; }

  [Required] [JsonPropertyName("comments")]
  public List<CommentDto> Comments;
  
  [JsonPropertyName("elapsedTime")]
  public TimeSpan ElapsedTime { get; set; }
};