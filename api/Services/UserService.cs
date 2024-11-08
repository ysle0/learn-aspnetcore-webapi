using api.Controllers;
using api.Interfaces;
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
  ) {
    return _mapper.Map<NewUserDto>(
      registerUserDto,
      opt => opt.AfterMap((src, dst) => dst.Token = token));
  }
}