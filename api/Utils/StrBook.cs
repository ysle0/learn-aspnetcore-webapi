namespace api.Utils;

public static class StrBook {
  public static class Auth {
    public static readonly string Unauthorized = "Unauthorized access";
    public static readonly string InvalidAuthInfo = "Invalid Username or Password";
  }

  public static class Stocks {
    public static readonly string NoExist = "Stock does not exist";
  }

  public static class Comments {
    public static readonly string NoExist = "Comment does not exist";
  }
}