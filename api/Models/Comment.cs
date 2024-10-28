namespace api.Models;

public class Comment {
  public int Id { get; set; }
  public string Title { get; set; } = "";
  public string Content { get; set; } = "";
  public DateTime CreatedOn { get; set; } = DateTime.Now;

  public int? StockId { get; set; }

  // Navigation property
  public Stock? Stock { get; set; }
}