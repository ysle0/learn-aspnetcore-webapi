using api.DTOs.Comment;
using api.Interfaces;
using api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/comment")]
[ApiController]
public class CommentController : ControllerBase {
  readonly ICommentRepository _commentRepository;
  readonly IStockRepository _stockRepository;
  readonly IMapper _mapper;

  public CommentController(
    IMapper mapper,
    IStockRepository stockRepository,
    ICommentRepository commentRepository
  ) {
    _mapper = mapper;
    _stockRepository = stockRepository;
    _commentRepository = commentRepository;
  }

  [HttpGet]
  [ProducesResponseType<List<Comment>>(StatusCodes.Status200OK)]
  public async Task<IActionResult> GetAllComments() =>
    Ok(await _commentRepository.GetAll());

  [HttpGet]
  [Route("{id:int}")]
  [ProducesResponseType<CommentDto>(StatusCodes.Status200OK)]
  public async ValueTask<IActionResult> GetById([FromRoute] int id) {
    Comment c = await _commentRepository.GetById(id);
    if (c == null) return NotFound();

    return Ok(_mapper.Map<CommentDto>(c));
  }

  [HttpPost("{stockId:int}")]
  public async Task<IActionResult>
    AddComment(
      [FromRoute] int stockId,
      [FromBody] CreateCommentDto createDto
    ) {
    bool isExist = await _stockRepository.ExistsStock(stockId);
    if (!isExist) return BadRequest("Stock does not exist");

    var c = _mapper.Map<Comment>(createDto);
    c.StockId = stockId;

    var newC = await _commentRepository.CreateNew(c);

    return CreatedAtAction(
      nameof(GetById),
      new { Id = c.Id, },
      _mapper.Map<CommentDto>(newC));
  }
}