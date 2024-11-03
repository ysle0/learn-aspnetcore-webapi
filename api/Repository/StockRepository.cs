using api.Controllers;
using api.Data;
using api.DTOs.Stock;
using api.Interfaces;
using api.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace api.Repository;

public class StockRepository(
  MySqlContext c,
  IMapper mp
) : IStockRepository {
  public async Task<List<StockDto>> GetAll() =>
    await c.Stocks
      .Include(e => e.Comments)
      .ProjectTo<StockDto>(mp.ConfigurationProvider)
      .ToListAsync();

  public async ValueTask<Stock?> GetById(int id) =>
    await c.Stocks.FindAsync(id);

  public async Task<bool> AddNew(Stock? s) {
    await c.Stocks.AddAsync(s);
    int cnt = await c.SaveChangesAsync();

    return cnt > 0;
  }

  public async Task<Stock?> Update(int id, StockUpdateDto u) {
    Stock? s = await c.Stocks.FirstOrDefaultAsync(e => e.Id == id);
    if (s == null) return null;

    s.CompanyName = u.CompanyName;
    s.Industry = u.Industry;
    s.Purchase = u.Purchase;
    s.Symbol = u.Symbol;
    s.MarketCap = u.MarketCap;
    s.LastDividend = u.LastDividend;

    await c.SaveChangesAsync();

    return s;
  }

  public async Task<bool> Delete(int id) {
    Stock? sToDelete = await c.Stocks.FirstOrDefaultAsync(e => e.Id == id);
    if (sToDelete == null) return false;

    c.Stocks.Remove(sToDelete);
    int cnt = await c.SaveChangesAsync();

    return cnt > 0;
  }

  public async Task<bool> ExistsStock(int id) =>
    await c.Stocks.AnyAsync(e => e.Id == id);
}