using api.Data;
using api.DTOs.Stock;
using api.Interfaces;
using api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

[Route("api/stock")]
[ApiController]
public class StockController(
  ApplicationDbContext dbContext,
  IStockRepository stockRepository,
  IMapper mapper
) : ControllerBase {
  [HttpGet]
  [ProducesResponseType<IEnumerable<StockDto>>(StatusCodes.Status200OK)]
  public async Task<IActionResult> GetAll() {
    var stocks = await stockRepository.GetAllAsync();
    var stockDtos = stocks.Select(mapper.Map<StockDto>);

    return Ok(stockDtos);
  }

  [HttpGet("{id:int}")]
  [ProducesResponseType<StockDto>(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetById([FromRoute] int id) {
    var stock = await stockRepository.GetByIdAsync(id);
    if (stock == null) {
      return NotFound();
    }

    var stockDto = mapper.Map<StockDto>(stock);
    return Ok(stockDto);
  }

  [HttpPost]
  [ProducesResponseType(StatusCodes.Status201Created)]
  public async Task<IActionResult> Create(
    [FromBody] StockCreateDto createDto
  ) {
    var stock = await stockRepository.CreateAsync(createDto);

    return CreatedAtAction(
      nameof(GetById),
      new { id = stock.Id, },
      stock
    );
  }

  [HttpPut]
  [Route("{id:int}")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  public async Task<IActionResult> Update(
    [FromRoute] int id,
    [FromBody] StockUpdateDto updateDto
  ) {
    var foundStock = await stockRepository.UpdateAsync(id, updateDto);
    if (foundStock == null) return NotFound();

    return Ok(mapper.Map<StockDto>(foundStock));
  }

  [HttpDelete]
  [Route("{id:int}")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  public async Task<IActionResult> Delete([FromRoute] int id) {
    var deletedStock = await stockRepository.DeleteAsync(id);
    if (deletedStock == null) return NotFound();

    return NoContent();
  }
}