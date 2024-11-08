using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api.Models;
using Microsoft.IdentityModel.Tokens;

namespace api.Services;

public class TokenService : ITokenService {
  readonly IConfiguration _config;
  readonly SymmetricSecurityKey _securityKey;

  public TokenService(IConfiguration config) {
    _config = config;

    var signingKeyAsBytes = Encoding.UTF8.GetBytes(_config["Jwt:SigningKey"]);
    _securityKey = new SymmetricSecurityKey(signingKeyAsBytes);
  }

  public string CreateToken(AppUser user) {
    var claims = new List<Claim> {
      new Claim(JwtRegisteredClaimNames.Email, user.Email),
      new Claim(JwtRegisteredClaimNames.GivenName, user.UserName),
    };

    var credentials = new SigningCredentials(
      _securityKey,
      SecurityAlgorithms.HmacSha512Signature);

    var tokenDescriptor = new SecurityTokenDescriptor {
      Subject = new ClaimsIdentity(claims),
      Expires = DateTime.Now.AddDays(7),
      SigningCredentials = credentials,
      Issuer = _config["Jwt:Issuer"],
      Audience = _config["Jwt:Audience"],
    };

    var tokenHandler = new JwtSecurityTokenHandler();
    var token = tokenHandler.CreateToken(tokenDescriptor);

    return tokenHandler.WriteToken(token);
  }
}