using api.Controllers;

namespace api.Interfaces;

public interface IUserService {
  NewUserDto MapToNewUserDto(RegisterUserDto registerUserDto, string token);
}