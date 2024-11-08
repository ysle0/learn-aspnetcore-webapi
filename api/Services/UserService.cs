using api.Controllers;
using api.Interfaces;
using api.Models;
using AutoMapper;

namespace api.Services;

public class UserService : IUserService {
  readonly IMapper _mapper;

  public UserService(IMapper mapper) {
    _mapper = mapper;
  }

  public NewUserDto MapToNewUserDto(
    RegisterUserDto registerUserDto,
    string token
  ) =>
    _mapper.Map<NewUserDto>(
      registerUserDto,
      opt => opt.AfterMap((src, dst) => {
        dst.Token = token;
      })
    );

  public NewUserDto MapToNewUserDto(AppUser user, string token) =>
    new() {
      Email = user.Email,
      UserName = user.UserName,
      Token = token
    };
}