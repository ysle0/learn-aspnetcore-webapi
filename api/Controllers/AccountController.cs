using api.Interfaces;
using api.Models;
using api.Services;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase {
  readonly IMapper _mapper;
  readonly UserManager<AppUser> _userManager;
  readonly ITokenService _tokenService;
  readonly IUserService _userService;

  public AccountController(
    IMapper mapper,
    UserManager<AppUser> userManager,
    ITokenService tokenService,
    IUserService userService
  ) {
    _mapper = mapper;
    _userManager = userManager;
    _tokenService = tokenService;
    _userService = userService;
  }

  [HttpPost("register")]
  [ProducesResponseType<NewUserDto>(StatusCodes.Status200OK)]
  public async Task<IActionResult> Register(
    [FromBody] RegisterUserDto registerUserDto
  ) {
    try {
      if (!ModelState.IsValid) return BadRequest(ModelState);

      AppUser appUser = new() {
        UserName = registerUserDto.UserName,
        Email = registerUserDto.EmailAddress,
      };

      IdentityResult createdUser
        = await _userManager.CreateAsync(appUser, registerUserDto.Password);
      if (!createdUser.Succeeded) {
        return StatusCode(
          StatusCodes.Status500InternalServerError,
          createdUser.Errors);
      }

      IdentityResult roleResult
        = await _userManager.AddToRoleAsync(appUser, "User");
      if (!roleResult.Succeeded) {
        return StatusCode(
          StatusCodes.Status500InternalServerError,
          roleResult.Errors);
      }

      var newUserDto = _userService.MapToNewUserDto(
        registerUserDto,
        _tokenService.CreateToken(appUser));
      return Ok(newUserDto);
    }
    catch (Exception e) {
      Console.WriteLine(e);
      return StatusCode(StatusCodes.Status500InternalServerError, e);
    }
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginDto loginDto) {
    if (!ModelState.IsValid) return BadRequest(ModelState);

    AppUser? user = await _userManager.Users
      .FirstOrDefaultAsync(e => e.UserName == loginDto.UserName);

    return Ok();
  }
}