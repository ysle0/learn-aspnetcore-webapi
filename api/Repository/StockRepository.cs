using api.Controllers;
using api.Data;
using api.DTOs.Stock;
using api.Interfaces;
using api.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace api.Repository;

public class StockRepository(
  ApplicationDbContext dbContext,
  IMapper mapper
) : IStockRepository {
  public async Task<List<Stock>> GetAllAsync() =>
    await dbContext.Stocks.ToListAsync();

  public async ValueTask<Stock?> GetByIdAsync(int id) =>
    await dbContext.Stocks.FindAsync(id);

  public async Task<Stock> CreateAsync(StockCreateDto createDto) {
    var stock = mapper.Map<Stock>(createDto);

    await dbContext.Stocks.AddAsync(stock);
    await dbContext.SaveChangesAsync();

    return stock;
  }

  public async Task<Stock> UpdateAsync(int id, StockUpdateDto updateDto) {
    var existingStock = await dbContext.Stocks
      .FirstOrDefaultAsync(s => s.Id == id);

    if (existingStock == null) return null;

    if (!string.IsNullOrEmpty(updateDto.Symbol))
      existingStock.Symbol = updateDto.Symbol;

    if (updateDto.LastDividend.HasValue)
      existingStock.LastDividend = updateDto.LastDividend.Value;

    if (!string.IsNullOrEmpty(updateDto.Industry))
      existingStock.Industry = updateDto.Industry;

    if (updateDto.Purchase.HasValue)
      existingStock.Purchase = updateDto.Purchase.Value;

    if (updateDto.MarketCap.HasValue)
      existingStock.MarketCap = updateDto.MarketCap.Value;

    if (!string.IsNullOrEmpty(updateDto.CompanyName))
      existingStock.CompanyName = updateDto.CompanyName;

    await dbContext.SaveChangesAsync();

    return existingStock;
  }

  public async Task<Stock> DeleteAsync(int id) {
    var deletedStock = await dbContext.Stocks
      .FirstOrDefaultAsync(s => s.Id == id);

    if (deletedStock == null) return null;

    dbContext.Stocks.Remove(deletedStock);
    await dbContext.SaveChangesAsync();

    return deletedStock;
  }
}