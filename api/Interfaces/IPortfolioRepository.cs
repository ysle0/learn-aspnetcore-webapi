using api.Models;

namespace api.Interfaces;

public interface IPortfolioRepository {
  Task<IList<Stock>?> GetUserPortfolio(AppUser user);
  Task<Portfolio> CreateUserPortfolio(Portfolio portfolio);
  Task<Portfolio> DeleteUserPortfolio(AppUser appUser, string symbol);
}