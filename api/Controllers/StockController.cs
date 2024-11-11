using System.Diagnostics;
using System.Net.Http.Headers;
using api.DTOs.Stock;
using api.Helpers;
using api.Interfaces;
using api.Models;
using api.Utils;
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
  [Authorize]
  [ProducesResponseType<IEnumerable<StockDto>>(StatusCodes.Status200OK)]
  public async Task<IActionResult> GetAll([FromQuery] QueryObject query) {
    if (!ModelState.IsValid) return BadRequest(ModelState);

    var watch = Stopwatch.StartNew();
    string cacheKey
      = StrBook.Stocks.MakeCacheKeyGetAllStocks(query.GetHashCode());

    string? cachedJson = await _redis.StringGetAsync(cacheKey);
    List<StockDto>? allStocks = null;
    if (string.IsNullOrWhiteSpace(cachedJson)) {
      allStocks = await _stockRepository.GetAll(query);

      string allStocksAsJson = JsonConvert.SerializeObject(allStocks);
      // Console.WriteLine($"all stocks as json:\n{allStocksAsJson}");

      var setCacheTask = _redis.StringSetAsync(cacheKey, allStocksAsJson);
      var expiredCacheTask
        = _redis.KeyExpireAsync(cacheKey, TimeSpan.FromSeconds(3600));

      await Task.WhenAll(setCacheTask, expiredCacheTask);

      watch.Stop();
      return Ok(allStocks);
    }

    allStocks = JsonConvert.DeserializeObject<List<StockDto>>(cachedJson);
    watch.Stop();
    return Ok(allStocks);
  }

  [HttpGet("{id:int}")]
  [Authorize]
  [ProducesResponseType<StockDto>(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetById([FromRoute] int id) {
    if (!ModelState.IsValid) return BadRequest(ModelState);

    var watch = Stopwatch.StartNew();
    string cacheKey = StrBook.Stocks.MakeCacheKeyGetOneStock(id);

    string? cachedJson = await _redis.StringGetAsync(cacheKey);
    Stock? stock = null;
    JObject result = null;

    if (string.IsNullOrWhiteSpace(cachedJson)) {
      stock = await _stockRepository.GetById(id);
      if (stock == null) return NotFound();

      string asJson = JsonConvert.SerializeObject(stock);
      var setTask = _redis.StringSetAsync(cacheKey, asJson);
      var expireTask
        = _redis.KeyExpireAsync(cacheKey, TimeSpan.FromSeconds(3600));

      await Task.WhenAll(setTask, expireTask);

      watch.Stop();
      result = new JObject {
        ["data"] = asJson,
        ["elapsed"] = watch.ElapsedMilliseconds,
      };

      return Ok(result);
    }

    stock = JsonConvert.DeserializeObject<Stock>(cachedJson);
    var resultStockDto = _mapper.Map<StockDto>(stock);
    watch.Stop();

    result = new JObject {
      ["data"] = JsonConvert.SerializeObject(resultStockDto),
      ["elapsedTime"] = watch.ElapsedMilliseconds,
    };
    return Ok(result);
  }

  [HttpPost]
  [Authorize]
  [ProducesResponseType(StatusCodes.Status201Created)]
  public async Task<IActionResult> Create(
    [FromBody] StockCreateDto createDto
  ) {
    if (!ModelState.IsValid) return BadRequest(ModelState);

    var stock = _mapper.Map<Stock>(createDto);
    await _stockRepository.Create(stock);

    return CreatedAtAction(
      nameof(GetById),
      new { id = stock.Id, },
      stock);
  }

  [HttpPut("{id:int}")]
  [Authorize]
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

  [HttpDelete("{id:int}")]
  [Authorize]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  public async Task<IActionResult> Delete([FromRoute] int id) {
    if (!ModelState.IsValid) return BadRequest(ModelState);

    bool isDeleteSuccess = await _stockRepository.Delete(id);
    if (!isDeleteSuccess) return NoContent();

    return NotFound();
  }
}