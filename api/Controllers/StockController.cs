using api.Data;
using api.DTOs.Stock;
using api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/stock")]
[ApiController]
public class StockController(
  ApplicationDbContext dbContext,
  IMapper mapper
) : ControllerBase {
  [HttpGet]
  [ProducesResponseType<IEnumerable<StockDto>>(StatusCodes.Status200OK)]
  public IActionResult GetAll() {
    var stocks = dbContext.Stocks
      .AsEnumerable()
      .Select(mapper.Map<StockDto>);

    return Ok(stocks);
  }

  [HttpGet("{id}")]
  [ProducesResponseType<StockDto>(StatusCodes.Status404NotFound)]
  public IActionResult GetById([FromRoute] int id) {
    var stock = dbContext.Stocks.Find(id);
    if (stock == null) {
      return NotFound();
    }

    var stockDto = mapper.Map<StockDto>(stock);
    return Ok(stockDto);
  }

  [HttpPost]
  [ProducesResponseType(StatusCodes.Status201Created)]
  public IActionResult Create(
    [FromBody] CreateStockRequestDto createStockRequestDto) {
    var stockModel = mapper.Map<Stock>(createStockRequestDto);
    dbContext.Stocks.Add(stockModel);
    dbContext.SaveChanges();

    return CreatedAtAction(
      nameof(GetById),
      new { id = stockModel.Id },
      stockModel);
  }
}