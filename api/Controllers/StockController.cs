using api.Data;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/stock")]
[ApiController]
public class StockController : ControllerBase {
  readonly ApplicationDbContext _dbContext;

  public StockController(ApplicationDbContext dbContext) {
    _dbContext = dbContext;
  }

  [HttpGet]
  public IActionResult GetAll() {
    var stocks = _dbContext.Stock
      .ToList()
      .Select(s => s.ToStockDto());

    return Ok(stocks);
  }

  [HttpGet("{id}")]
  public IActionResult GetById([FromRoute] int id) {
    var stock = _dbContext.Stock.Find(id);
    if (stock == null) {
      return NotFound();
    }

    return Ok(stock.ToStockDto());
  }
}