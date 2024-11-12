using api.Data;
using api.Interfaces;
using api.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace api.Repository;

public class PortfolioRepository : IPortfolioRepository {
  readonly AppDbContext _ctx;
  readonly IMapper _mapper;

  public PortfolioRepository(
    AppDbContext dbContext,
    IMapper mapper
  ) {
    _ctx = dbContext;
    _mapper = mapper;
  }

  public async Task<IList<Stock>?> GetUserPortfolio(AppUser user) {
    return await _ctx.Portfolios
      .Where(p => p.AppUserId == user.Id)
      .ProjectTo<Stock>(_mapper.ConfigurationProvider)
      .ToListAsync();
  }

  public async Task<Portfolio> CreateUserPortfolio(Portfolio portfolio) {
    await _ctx.Portfolios.AddAsync(portfolio);
    await _ctx.SaveChangesAsync();

    return portfolio;
  }

  public async Task<Portfolio> DeleteUserPortfolio(
    AppUser appUser,
    string symbol
  ) {
    var portfolioModel = await _ctx.Portfolios
      .FirstOrDefaultAsync(x =>
        x.AppUserId == appUser.Id &&
        x.Stock.Symbol.ToLower() == symbol.ToLower()
      );

    if (portfolioModel == null) return null;

    _ctx.Portfolios.Remove(portfolioModel);
    await _ctx.SaveChangesAsync();

    return portfolioModel;
  }
}