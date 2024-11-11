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

  public async Task<IList<Stock>> GetUserPortfolio(AppUser user) {
    return await _ctx.Portfolios
      .Where(p => p.AppUserId == user.Id)
      .ProjectTo<Stock>(_mapper.ConfigurationProvider)
      .ToListAsync();
  }
}