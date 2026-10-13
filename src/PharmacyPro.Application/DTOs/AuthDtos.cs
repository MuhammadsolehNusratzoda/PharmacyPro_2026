using System.ComponentModel.DataAnnotations;
using PharmacyPro.Domain.Constants;

namespace PharmacyPro.Application.DTOs;

public class RegisterDto
{
    [Required, MaxLength(100)] public string FullName { get; set; } = null!;
    [Required, EmailAddress] public string Email { get; set; } = null!;
    [Required, MinLength(8)] public string Password { get; set; } = null!;
    [Required] public string Role { get; set; } = Roles.Pharmacist;
}

public class LoginDto
{
    [Required, EmailAddress] public string Email { get; set; } = null!;
    [Required] public string Password { get; set; } = null!;
}

public class RefreshRequestDto
{
    [Required] public string RefreshToken { get; set; } = null!;
}

public class ChangePasswordDto
{
    [Required] public string CurrentPassword { get; set; } = null!;
    [Required, MinLength(8)] public string NewPassword { get; set; } = null!;
}

public record UserInfoDto(string Id, string FullName, string Email, IList<string> Roles);

public record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    UserInfoDto User);