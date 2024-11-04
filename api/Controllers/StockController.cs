using api.DTOs.Stock;
using api.Interfaces;
using api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/stock")]
[ApiController]
public class StockController : ControllerBase {
  readonly IStockRepository _stockRepository;
  readonly IMapper _mapper;

  public StockController(
    IStockRepository stockRepository,
    IMapper mapper
  ) {
    _mapper = mapper;
    _stockRepository = stockRepository;
  }

  [HttpGet]
  [ProducesResponseType<IEnumerable<StockDto>>(StatusCodes.Status200OK)]
  public async Task<IActionResult> GetAll() =>
    Ok(await _stockRepository.GetAll());

  [HttpGet("{id:int}")]
  [ProducesResponseType<StockDto>(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetById([FromRoute] int id) {
    Stock? stock = await _stockRepository.GetById(id);
    if (stock == null) return NotFound();

    return Ok(_mapper.Map<StockDto>(stock));
  }

  [HttpPost]
  [ProducesResponseType(StatusCodes.Status201Created)]
  public async Task<IActionResult> Create(
    [FromBody] StockCreateDto createDto
  ) {
    var stock = _mapper.Map<Stock>(createDto);
    await _stockRepository.AddNew(stock);

    return CreatedAtAction(
      nameof(GetById),
      new { id = stock.Id, },
      stock);
  }

  [HttpPut]
  [Route("{id:int}")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  public async Task<IActionResult> Update(
    [FromRoute] int id,
    [FromBody] StockUpdateDto updateDto
  ) {
    var updatedStock = await _stockRepository.Update(id, updateDto);
    if (updatedStock == null) return NotFound();

    return Ok(updatedStock);
  }

  [HttpDelete]
  [Route("{id:int}")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  public async Task<IActionResult> Delete([FromRoute] int id) {
    bool isDeleteSuccess = await _stockRepository.Delete(id);
    if (!isDeleteSuccess) return NoContent();

    return NotFound();
  }
}