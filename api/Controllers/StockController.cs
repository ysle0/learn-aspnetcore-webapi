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

  [HttpGet("{id:int}")]
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
    [FromBody] StockCreateDto stockCreateDto
  ) {
    var stockModel = mapper.Map<Stock>(stockCreateDto);
    dbContext.Stocks.Add(stockModel);
    dbContext.SaveChanges();

    return CreatedAtAction(
      nameof(GetById),
      new { id = stockModel.Id, },
      stockModel);
  }

  [HttpPut]
  [Route("{id:int}")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  public IActionResult Update(
    [FromRoute] int id,
    [FromBody] StockUpdateDto stockUpdateDto
  ) {
    Stock? stockModel = dbContext.Stocks.FirstOrDefault(s => s.Id == id);
    if (stockModel == null) return NotFound();

    stockModel.Symbol = stockUpdateDto.Symbol;
    stockModel.LastDividend = stockUpdateDto.LastDividend;
    stockModel.Industry = stockUpdateDto.Industry;
    stockModel.Purchase = stockUpdateDto.Purchase;
    stockModel.MarketCap = stockUpdateDto.MarketCap;
    stockModel.CompanyName = stockUpdateDto.CompanyName;

    dbContext.SaveChanges();

    return Ok(mapper.Map<StockDto>(stockModel));
  }

  [HttpDelete]
  [Route("{id:int}")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  public IActionResult Delete(
    [FromRoute] int id
  ) {
    var stockModel = dbContext.Stocks.FirstOrDefault(s => s.Id == id);
    if (stockModel == null) return NotFound();

    dbContext.Stocks.Remove(stockModel);
    dbContext.SaveChanges();

    return NoContent();
  }
}