using api.Controllers;
using api.Models;

namespace api.Interfaces;

public interface IUserService {
  NewUserDto MapToNewUserDto(RegisterUserDto registerUserDto, string token);
  NewUserDto MapToNewUserDto(AppUser user, string token);
}