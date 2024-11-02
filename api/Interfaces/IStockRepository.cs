using api.Controllers;
using api.DTOs.Stock;
using api.Models;

namespace api.Interfaces;

public interface IStockRepository {
  Task<List<Stock>> GetAllAsync();
  ValueTask<Stock?> GetByIdAsync(int id);
  Task<Stock> CreateAsync(StockCreateDto createDto);
  Task<Stock> UpdateAsync(int id, StockUpdateDto updateDto);
  Task<Stock> DeleteAsync(int id);
}