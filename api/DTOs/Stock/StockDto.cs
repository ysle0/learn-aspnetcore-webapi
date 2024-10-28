namespace api.DTOs.Stock;

public class StockDto
{

    public int Id { get; set; }

    public string CompanyName { get; set; } = "";

    public string Symbol { get; set; } = "";

    public decimal Purchase { get; set; }

    public decimal LastDividend { get; set; }

    public string Industry { get; set; } = "";

    public long MarketCap { get; set; }

    // Comments
}