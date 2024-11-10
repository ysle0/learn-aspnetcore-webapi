using System.Diagnostics;
using api.DTOs.Comment;
using api.Interfaces;
using api.Models;
using api.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentController : ControllerBase {
  readonly ICommentRepository _commentRepository;
  readonly IStockRepository _stockRepository;
  readonly IMapper _mapper;
  readonly IDatabase _redis;

  public CommentController(
    IMapper mapper,
    IConnectionMultiplexer muxer,
    IStockRepository stockRepository,
    ICommentRepository commentRepository
  ) {
    _mapper = mapper;
    _redis = muxer.GetDatabase();

    _stockRepository = stockRepository;
    _commentRepository = commentRepository;
  }

  [HttpGet]
  [Authorize]
  [ProducesResponseType<List<Comment>>(StatusCodes.Status200OK)]
  public async Task<IActionResult> GetAllComments() {
    if (!ModelState.IsValid) return BadRequest(ModelState);

    var watch = Stopwatch.StartNew();
    string cacheKey = StrBook.Comments.MakeCacheKeyGetAllComments();
    JObject result;
    string? cachedJson = await _redis.StringGetAsync(cacheKey);

    if (string.IsNullOrWhiteSpace(cachedJson)) {
      var allComments = await _commentRepository.GetAll();
      string asJson = JsonConvert.SerializeObject(allComments);
      var setTask = _redis.StringSetAsync(cacheKey, asJson);
      var expireTask
        = _redis.KeyExpireAsync(cacheKey, TimeSpan.FromSeconds(3600));

      await Task.WhenAll(setTask, expireTask);

      watch.Stop();
      result = new JObject {
        ["data"] = asJson,
        ["elapsedTime"] = watch.ElapsedMilliseconds
      };

      return Ok(result);
    }

    watch.Stop();
    result = new JObject {
      ["data"] = cachedJson,
      ["elapsedTime"] = watch.ElapsedMilliseconds
    };

    return Ok(result);
  }

  [HttpGet("{id:int}")]
  [Authorize]
  [ProducesResponseType<CommentDto>(StatusCodes.Status200OK)]
  public async ValueTask<IActionResult> GetById([FromRoute] int id) {
    if (!ModelState.IsValid) return BadRequest(ModelState);

    var watch = Stopwatch.StartNew();
    string cacheKey = StrBook.Comments.MakeCacheKeyGetOneComment(id);
    string? cachedJson = await _redis.StringGetAsync(cacheKey);
    CommentDto commentDto = null;
    JObject result = null;
    if (string.IsNullOrWhiteSpace(cachedJson)) {
      var comment = await _commentRepository.GetById(id);
      if (comment == null) return NotFound();
      
      var asDto = _mapper.Map<CommentDto>(comment);
      string asJson = JsonConvert.SerializeObject(asDto);

      var setTask = _redis.StringSetAsync(cacheKey, asJson);
      var expireTask
        = _redis.KeyExpireAsync(cacheKey, TimeSpan.FromSeconds(3600));

      await Task.WhenAll(setTask, expireTask);

      watch.Stop();
      result = new JObject {
        ["data"] = asJson,
        ["elapsedTime"] = watch.ElapsedMilliseconds
      };

      return Ok(result);
    }

    watch.Stop();
    result = new JObject {
      ["data"] = cachedJson,
      ["elapsedTime"] = watch.ElapsedMilliseconds
    };

    return Ok(result);
  }

  [HttpPost("{stockId:int}")]
  public async Task<IActionResult>
    AddComment(
      [FromRoute] int stockId,
      [FromBody] CreateCommentDto createDto
    ) {
    if (!ModelState.IsValid) return BadRequest(ModelState);

    bool isExist = await _stockRepository.ExistsStock(stockId);
    if (!isExist) return BadRequest(StrBook.Stocks.NoExist);

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
    if (deletedComment == null) return NotFound(StrBook.Comments.NoExist);

    return Ok();
  }
}