using api.Controllers;
using api.Models;

namespace api.Utils;

public static class StrBook {
  public static class Auth {
    public static readonly string Unauthorized = "Unauthorized access";

    public static readonly string InvalidAuthInfo
      = "Invalid Username or Password";
  }

  public static class Stocks {
    public static readonly string NoExist = "Stock does not exist";

    public static string MakeCacheKeyGetAllStocks(int queryObjectHash) =>
      $"{nameof(Stock)}:{nameof(StockController.GetAll)}:{queryObjectHash}";

    public static string MakeCacheKeyGetOneStock(int id) =>
      $"{nameof(Stock)}:{nameof(StockController.GetById)}:{id}";
  }

  public static class Comments {
    public static readonly string NoExist = "Comment does not exist";

    public static string MakeCacheKeyGetAllComments() =>
      $"{nameof(Comment)}:{nameof(CommentController.GetAllComments)}";

    public static string MakeCacheKeyGetOneComment(int id) =>
      $"{nameof(Comment)}:{nameof(CommentController.GetById)}:{id}";
  }

  public static class Portfolio {
    public static string MakeCacheKeyGetUserPortfolios(string appUserId) =>
      $"{nameof(Models.Portfolio)}:{nameof(PortfolioController.GetUserPortfolio)}:{appUserId}";

    public static readonly string NoStocksFound = "No stocks found";
  }
}