using System.Diagnostics;
using api.Extensions;
using api.Interfaces;
using api.Models;
using api.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PortfolioController : ControllerBase {
  readonly IMapper _mapper;
  readonly UserManager<AppUser> _userManager;

  readonly IStockRepository _stockRepository;
  readonly IPortfolioRepository _portfolioRepository;

  readonly IDatabase _redis;

  public PortfolioController(
    IMapper mapper,
    IConnectionMultiplexer muxer,
    UserManager<AppUser> userManager,
    IStockRepository stockRepository,
    IPortfolioRepository portfolioRepository
  ) {
    _mapper = mapper;
    _userManager = userManager;
    _redis = muxer.GetDatabase();
    _stockRepository = stockRepository;
    _portfolioRepository = portfolioRepository;
  }

  [HttpGet]
  [Authorize]
  [ProducesResponseType<IEnumerable<Stock>>(StatusCodes.Status200OK)]
  public async Task<IActionResult> GetUserPortfolio() {
    string userName = User.GetUsername();
    AppUser? appUser = await _userManager.FindByNameAsync(userName);
    if (appUser == null) return BadRequest();

    var watch = Stopwatch.StartNew();
    string cacheKey
      = StrBook.Portfolio.MakeCacheKeyGetUserPortfolios(appUser.Id);
    string? cachedValue = await _redis.StringGetAsync(cacheKey);
    IList<Stock>? userPortfolios = null;
    JObject result;
    if (string.IsNullOrWhiteSpace(cachedValue)) {
      userPortfolios = await _portfolioRepository.GetUserPortfolio(appUser);
      string asJson = JsonConvert.SerializeObject(userPortfolios);

      var setTask = _redis.StringSetAsync(cacheKey, asJson);
      var expireTask
        = _redis.KeyExpireAsync(cacheKey, TimeSpan.FromSeconds(3600));

      await Task.WhenAll(setTask, expireTask);

      watch.Stop();
      result = new JObject {
        ["data"] = asJson,
        ["elapsedTime"] = watch.ElapsedMilliseconds,
      };

      return Ok(result);
    }

    watch.Stop();
    result = new JObject {
      ["data"] = cachedValue,
      ["elapsedTime"] = watch.ElapsedMilliseconds,
    };

    return Ok(result);
  }

  [HttpPost]
  [Authorize]
  [ProducesResponseType(StatusCodes.Status201Created)]
  public async Task<IActionResult> CreateUserPortfolio(string symbol) {
    if (string.IsNullOrWhiteSpace(symbol)) return BadRequest();

    string userName = User.GetUsername();
    AppUser? appUser = await _userManager.FindByNameAsync(userName);
    if (appUser == null) return BadRequest();

    Stock? stock = await _stockRepository.GetBySymbol(symbol);
    if (stock == null)
      return StatusCode(StatusCodes.Status500InternalServerError);

    var userPortfolio = await _portfolioRepository.GetUserPortfolio(appUser);
    if (userPortfolio.Any(e => e.Symbol.ToLower() == symbol.ToLower()))
      return BadRequest("Stock already exists");

    var portfolioModel = new Portfolio {
      StockId = stock.Id,
      AppUserId = appUser.Id,
    };

    await _portfolioRepository.CreateUserPortfolio(portfolioModel);

    if (portfolioModel == null) {
      return StatusCode(
        StatusCodes.Status500InternalServerError,
        "Portfolio could not be created");
    }

    string cacheKey
      = StrBook.Portfolio.MakeCacheKeyGetUserPortfolios(appUser.Id);
    _redis.KeyExpire(
      cacheKey, TimeSpan.FromSeconds(0), CommandFlags.FireAndForget);

    return Created();
  }


  [HttpDelete]
  [Authorize]
  public async Task<IActionResult> Delete(string symbol) {
    var userName = base.User.GetUsername();
    AppUser? appUser = await _userManager.FindByNameAsync(userName);
    if (appUser == null) return BadRequest();

    IList<Stock>? userPortfolio
      = await _portfolioRepository.GetUserPortfolio(appUser);
    if (userPortfolio == null) return BadRequest();

    List<Stock> filteredStocks = userPortfolio
      .Where(e => e.Symbol.ToLower() == symbol.ToLower())
      .ToList();

    if (filteredStocks.Count == 0)
      return BadRequest(StrBook.Portfolio.NoStocksFound);

    await _portfolioRepository.DeleteUserPortfolio(appUser, symbol);
    
    string cacheKey
      = StrBook.Portfolio.MakeCacheKeyGetUserPortfolios(appUser.Id);
    _redis.KeyExpire(
      cacheKey, TimeSpan.FromSeconds(0), CommandFlags.FireAndForget);

    return Ok();
  }
}