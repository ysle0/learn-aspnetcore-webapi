using api.DTOs.Comment;
using api.Interfaces;
using api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
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
  public async Task<IActionResult> GetAllComments() {
    if (!ModelState.IsValid) return BadRequest(ModelState);

    var allComments = await _commentRepository.GetAll();
    return Ok(allComments);
  }

  [HttpGet]
  [Route("{id:int}")]
  [ProducesResponseType<CommentDto>(StatusCodes.Status200OK)]
  public async ValueTask<IActionResult> GetById([FromRoute] int id) {
    if (!ModelState.IsValid) return BadRequest(ModelState);

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
    if (!ModelState.IsValid) return BadRequest(ModelState);

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

  [HttpPut("{id:int}")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  public async Task<IActionResult> Update(
    [FromRoute] int id,
    [FromBody] UpdateCommentDto dto
  ) {
    if (!ModelState.IsValid) return BadRequest(ModelState);

    var updatedComment
      = await _commentRepository.Update(id, _mapper.Map<Comment>(dto));
    if (updatedComment == null) return NotFound();

    return NoContent();
  }

  [HttpDelete("{id:int}")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public async Task<IActionResult> Delete([FromRoute] int id) {
    if (!ModelState.IsValid) return BadRequest(ModelState);

    var deletedComment = await _commentRepository.Delete(id);
    if (deletedComment == null) return NotFound("Comment not found");

    return Ok();
  }
}