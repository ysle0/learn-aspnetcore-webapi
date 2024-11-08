using System.ComponentModel.DataAnnotations;

namespace api.Controllers;

public class RegisterUserDto {
  [Required] public string? UserName { get; set; }

  [Required] [EmailAddress] public string? EmailAddress { get; set; }
  [Required] public string? Password { get; set; }
}