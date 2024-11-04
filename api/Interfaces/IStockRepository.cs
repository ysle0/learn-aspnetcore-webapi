using api.Controllers;
using api.DTOs.Stock;
using api.Helpers;
using api.Models;

namespace api.Interfaces;

public interface IStockRepository {
  Task<List<StockDto>> GetAll(QueryObject query);
  ValueTask<Stock?> GetById(int id);
  Task<bool> AddNew(Stock? s);
  Task<Stock?> Update(int id, StockUpdateDto u);
  Task<bool> Delete(int id);
  Task<bool> ExistsStock(int id);
}