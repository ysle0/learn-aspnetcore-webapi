using api.Controllers;
using api.Data;
using api.DTOs.Stock;
using api.Helpers;
using api.Interfaces;
using api.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace api.Repository;

public class StockRepository : IStockRepository {
  readonly MySqlContext _ctx;
  readonly IMapper _mapper;

  public StockRepository(MySqlContext ctx, IMapper mapper) {
    _ctx = ctx;
    _mapper = mapper;
  }

  public async Task<List<StockDto>> GetAll(QueryObject query) {
    var stocks = _ctx.Stocks
      .Include(e => e.Comments)
      .ProjectTo<StockDto>(_mapper.ConfigurationProvider)
      .AsQueryable();

    if (!string.IsNullOrWhiteSpace(query.CompanyName)) {
      stocks = stocks.Where(e => e.CompanyName.Contains(query.CompanyName));
    }

    if (!string.IsNullOrWhiteSpace(query.Symbol)) {
      stocks = stocks.Where(e => e.Symbol.Contains(query.Symbol));
    }

    return await stocks.ToListAsync();
  }

  public async ValueTask<Stock?> GetById(int id) =>
    await _ctx.Stocks.FindAsync(id);

  public async Task<bool> AddNew(Stock? s) {
    await _ctx.Stocks.AddAsync(s);
    int cnt = await _ctx.SaveChangesAsync();

    return cnt > 0;
  }

  public async Task<Stock?> Update(int id, StockUpdateDto u) {
    Stock? s = await _ctx.Stocks.FirstOrDefaultAsync(e => e.Id == id);
    if (s == null) return null;

    s.CompanyName = u.CompanyName;
    s.Industry = u.Industry;
    s.Purchase = u.Purchase;
    s.Symbol = u.Symbol;
    s.MarketCap = u.MarketCap;
    s.LastDividend = u.LastDividend;

    await _ctx.SaveChangesAsync();

    return s;
  }

  public async Task<bool> Delete(int id) {
    Stock? sToDelete = await _ctx.Stocks.FirstOrDefaultAsync(e => e.Id == id);
    if (sToDelete == null) return false;

    _ctx.Stocks.Remove(sToDelete);
    int cnt = await _ctx.SaveChangesAsync();

    return cnt > 0;
  }

  public async Task<bool> ExistsStock(int id) =>
    await _ctx.Stocks.AnyAsync(e => e.Id == id);
}