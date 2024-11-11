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
  public async Task<IActionResult> GetUserPortfolio([FromQuery] object qo) {
    string userName = User.GetUsername();
    AppUser? appUser = await _userManager.FindByNameAsync(userName);
    if (appUser == null) return BadRequest();

    IList<Stock> userPortfolios
      = await _portfolioRepository.GetUserPortfolio(appUser);

    return Ok(userPortfolios);
  }
}