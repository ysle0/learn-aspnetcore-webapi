using System.ComponentModel.DataAnnotations;
using api.DTOs.Comment;

namespace api.DTOs.Stock;

public struct StockDto {
  [Required] [Length(4, 280)] public string CompanyName { get; set; }
  [Required] [Length(4, 280)] public string Symbol { get; set; }
  [Required] public decimal Purchase { get; set; }
  [Required] public decimal LastDividend { get; set; }
  [Required] [Length(4, 280)] public string Industry { get; set; }
  [Required] public long MarketCap { get; set; }
  List<CommentDto> Comments;
};