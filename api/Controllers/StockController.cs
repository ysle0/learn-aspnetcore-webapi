using api.Data;
using api.DTOs.Stock;
using api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

[Route("api/stock")]
[ApiController]
public class StockController(
  ApplicationDbContext dbContext,
  IMapper mapper
) : ControllerBase {
  [HttpGet]
  [ProducesResponseType<IEnumerable<StockDto>>(StatusCodes.Status200OK)]
  public async Task<IActionResult> GetAll() {
    var stocks = await dbContext.Stocks.ToListAsync();
    var stockDtos = stocks.Select(mapper.Map<StockDto>);

    return Ok(stockDtos);
  }

  [HttpGet("{id:int}")]
  [ProducesResponseType<StockDto>(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetById([FromRoute] int id) {
    var stock = await dbContext.Stocks.FindAsync(id);
    if (stock == null) {
      return NotFound();
    }

    var stockDto = mapper.Map<StockDto>(stock);
    return Ok(stockDto);
  }

  [HttpPost]
  [ProducesResponseType(StatusCodes.Status201Created)]
  public async Task<IActionResult> Create(
    [FromBody] StockCreateDto stockCreateDto
  ) {
    var stockModel = mapper.Map<Stock>(stockCreateDto);
    await dbContext.Stocks.AddAsync(stockModel);
    await dbContext.SaveChangesAsync();

    return CreatedAtAction(
      nameof(GetById),
      new { id = stockModel.Id, },
      stockModel);
  }

  [HttpPut]
  [Route("{id:int}")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  public async Task<IActionResult> Update(
    [FromRoute] int id,
    [FromBody] StockUpdateDto updateDto
  ) {
    var stockModel
      = await dbContext.Stocks.FirstOrDefaultAsync(s => s.Id == id);

    if (stockModel == null) return NotFound();

    if (!string.IsNullOrEmpty(updateDto.Symbol))
      stockModel.Symbol = updateDto.Symbol;

    if (updateDto.LastDividend.HasValue)
      stockModel.LastDividend = updateDto.LastDividend.Value;

    if (!string.IsNullOrEmpty(updateDto.Industry))
      stockModel.Industry = updateDto.Industry;

    if (updateDto.Purchase.HasValue)
      stockModel.Purchase = updateDto.Purchase.Value;

    if (updateDto.MarketCap.HasValue)
      stockModel.MarketCap = updateDto.MarketCap.Value;

    if (!string.IsNullOrEmpty(updateDto.CompanyName))
      stockModel.CompanyName = updateDto.CompanyName;

    await dbContext.SaveChangesAsync();

    return Ok(mapper.Map<StockDto>(stockModel));
  }

  [HttpDelete]
  [Route("{id:int}")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  public async Task<IActionResult> Delete([FromRoute] int id) {
    var stockModel
      = await dbContext.Stocks.FirstOrDefaultAsync(s => s.Id == id);
    if (stockModel == null) return NotFound();

    dbContext.Stocks.Remove(stockModel);
    await dbContext.SaveChangesAsync();

    return NoContent();
  }
}