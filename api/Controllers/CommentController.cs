using api.DTOs.Comment;
using api.Interfaces;
using api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/comment")]
[ApiController]
public class CommentController(
  ICommentRepository _rc,
  IStockRepository _rs,
  IMapper _mp
) : ControllerBase {
  [HttpGet]
  [ProducesResponseType<List<Comment>>(StatusCodes.Status200OK)]
  public async Task<IActionResult> GetAllComments() => Ok(await _rc.GetAll());

  [HttpGet]
  [Route("{id:int}")]
  [ProducesResponseType<CommentDto>(StatusCodes.Status200OK)]
  public async ValueTask<IActionResult> GetById([FromRoute] int id) {
    Comment c = await _rc.GetById(id);
    if (c == null) return NotFound();

    return Ok(_mp.Map<CommentDto>(c));
  }

  [HttpPost("{stockId:int}")]
  public async Task<IActionResult>
    AddComment(
      [FromRoute] int stockId,
      [FromBody] CreateCommentDto createDto
    ) {
    bool isExist = await _rs.ExistsStock(stockId);
    if (!isExist) return BadRequest("Stock does not exist");

    var c = _mp.Map<Comment>(createDto);
    c.StockId = stockId;

    var newC = await _rc.CreateNew(c);

    return CreatedAtAction(
      nameof(GetById),
      new { Id = c.Id, },
      _mp.Map<CommentDto>(newC));
  }
}