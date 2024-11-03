using api.DTOs.Stock;
using api.Interfaces;
using api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/stock")]
[ApiController]
public class StockController(
  IStockRepository r,
  IMapper mp
) : ControllerBase {
  [HttpGet]
  [ProducesResponseType<IEnumerable<StockDto>>(StatusCodes.Status200OK)]
  public async Task<IActionResult> GetAll() {
    var s = await r.GetAll();
    return Ok(s);
  }

  [HttpGet("{id:int}")]
  [ProducesResponseType<StockDto>(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetById([FromRoute] int id) {
    Stock? s = await r.GetById(id);
    if (s == null) return NotFound();

    return Ok(mp.Map<StockDto>(s));
  }

  [HttpPost]
  [ProducesResponseType(StatusCodes.Status201Created)]
  public async Task<IActionResult> Create(
    [FromBody] StockCreateDto createDto
  ) {
    var s = mp.Map<Stock>(createDto);
    await r.AddNew(s);

    return CreatedAtAction(nameof(GetById), new { id = s.Id, }, s);
  }

  [HttpPut]
  [Route("{id:int}")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  public async Task<IActionResult> Update(
    [FromRoute] int id,
    [FromBody] StockUpdateDto updateDto
  ) {
    var s = await r.Update(id, updateDto);
    return s == null ? NotFound() : Ok(s);
  }

  [HttpDelete]
  [Route("{id:int}")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  public async Task<IActionResult> Delete([FromRoute] int id) {
    bool suc = await r.Delete(id);
    return !suc ? NotFound() : NoContent();
  }
}