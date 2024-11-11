using api.Models;

namespace api.Interfaces;

public interface IPortfolioRepository {
  Task<IList<Stock>> GetUserPortfolio(AppUser user);
}