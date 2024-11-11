using api.Extensions;
using api.Interfaces;
using api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

    IList<Stock> userPortfolios
      = await _portfolioRepository.GetUserPortfolio(appUser);

    return Ok(userPortfolios);
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

    return Created();
  }
}