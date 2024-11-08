using api.Models;
using api.Services;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase {
  readonly IMapper _mapper;
  readonly UserManager<AppUser> _userManager;
  readonly ITokenService _tokenService;

  public AccountController(
    IMapper mapper,
    UserManager<AppUser> userManager,
    ITokenService tokenService
  ) {
    _mapper = mapper;
    _userManager = userManager;
    _tokenService = tokenService;
  }

  [HttpPost("register")]
  public async Task<IActionResult> Register(
    [FromBody] RegisterUserDto registerUserDto) {
    try {
      if (!ModelState.IsValid) return BadRequest(ModelState);

      AppUser appUser = new() {
        UserName = registerUserDto.UserName,
        Email = registerUserDto.EmailAddress,
      };

      IdentityResult createdUser
        = await _userManager.CreateAsync(appUser, registerUserDto.Password);

      if (createdUser.Succeeded) {
        var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
        if (roleResult.Succeeded) {
          var newUserDto = _mapper.Map<NewUserDto>(
            registerUserDto,
            opt => {
              const string tokenField = nameof(NewUserDto.Token);
              opt.AfterMap((src, dst) =>
                dst.Token = _tokenService.CreateToken(appUser));
              // opt.Items[tokenField] = _tokenService.CreateToken(appUser);
            });

          return Ok(newUserDto);
        }

        return StatusCode(
          StatusCodes.Status500InternalServerError,
          roleResult.Errors);
      }

      return StatusCode(
        StatusCodes.Status500InternalServerError,
        createdUser.Errors);
    }
    catch (Exception e) {
      Console.WriteLine(e);
      return StatusCode(StatusCodes.Status500InternalServerError, e);
    }
  }
}