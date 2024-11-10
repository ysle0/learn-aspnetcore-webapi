using System.Diagnostics;
using System.Net.Http.Headers;
using api.DTOs.Stock;
using api.Helpers;
using api.Interfaces;
using api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockController : ControllerBase {
  readonly IStockRepository _stockRepository;
  readonly IMapper _mapper;
  readonly IDatabase _redis;

  public StockController(
    IMapper mapper,
    HttpClient httpClient,
    IConnectionMultiplexer muxer,
    IStockRepository stockRepository
  ) {
    _mapper = mapper;
    // _httpClient = httpClient;
    // _httpClient
    //   .DefaultRequestHeaders
    //   .UserAgent
    //   .Add(
    //     new ProductInfoHeaderValue("learnAspNetCoreWebApiApp", "1.0"));
    _redis = muxer.GetDatabase();

    _stockRepository = stockRepository;
  }

  [HttpGet]
  [ProducesResponseType<IEnumerable<StockDto>>(StatusCodes.Status200OK)]
  [Authorize]
  public async Task<IActionResult> GetAll([FromQuery] QueryObject query) {
    if (!ModelState.IsValid) return BadRequest(ModelState);

    var watch = Stopwatch.StartNew();

    const string CACHE_KEY = $"{nameof(StockDto)}:{nameof(GetAll)}";

    string? cachedJson = await _redis.StringGetAsync(CACHE_KEY);
    List<StockDto>? allStocks = null;
    if (string.IsNullOrWhiteSpace(cachedJson)) {
      allStocks = await _stockRepository.GetAll(query);

      string allStocksAsJson = JsonConvert.SerializeObject(allStocks);
      // Console.WriteLine($"all stocks as json:\n{allStocksAsJson}");

      var setCacheTask = _redis.StringSetAsync(CACHE_KEY, allStocksAsJson);
      var expiredCacheTask
        = _redis.KeyExpireAsync(CACHE_KEY, TimeSpan.FromSeconds(3600));

      await Task.WhenAll(setCacheTask, expiredCacheTask);

      watch.Stop();
      return Ok(allStocks);
    }

    allStocks = JsonConvert.DeserializeObject<List<StockDto>>(cachedJson);
    return Ok(allStocks);
  }

  [HttpGet("{id:int}")]
  [ProducesResponseType<StockDto>(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetById([FromRoute] int id) {
    if (!ModelState.IsValid) return BadRequest(ModelState);

    Stock? stock = await _stockRepository.GetById(id);
    if (stock == null) return NotFound();

    return Ok(_mapper.Map<StockDto>(stock));
  }

  [HttpPost]
  [ProducesResponseType(StatusCodes.Status201Created)]
  public async Task<IActionResult> Create(
    [FromBody] StockCreateDto createDto
  ) {
    if (!ModelState.IsValid) return BadRequest(ModelState);

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
    if (!ModelState.IsValid) return BadRequest(ModelState);

    var updatedStock = await _stockRepository.Update(id, updateDto);
    if (updatedStock == null) return NotFound();

    return Ok(updatedStock);
  }

  [HttpDelete]
  [Route("{id:int}")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  public async Task<IActionResult> Delete([FromRoute] int id) {
    if (!ModelState.IsValid) return BadRequest(ModelState);

    bool isDeleteSuccess = await _stockRepository.Delete(id);
    if (!isDeleteSuccess) return NoContent();

    return NotFound();
  }
}