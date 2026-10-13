using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PharmacyPro.Application.DTOs;
using PharmacyPro.Application.Interfaces;
using PharmacyPro.Domain.Constants;

namespace PharmacyPro.API.Controllers;

[Route("api/auth")]
public class AuthController(IAuthService auth) : BaseApiController
{
    [HttpPost("register")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Register(RegisterDto dto) =>
        Result(await auth.RegisterAsync(dto));

    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Login(LoginDto dto) =>
        Result(await auth.LoginAsync(dto));

    [HttpPost("refresh")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Refresh(RefreshRequestDto dto) =>
        Result(await auth.RefreshAsync(dto.RefreshToken));

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(RefreshRequestDto dto) =>
        Result(await auth.LogoutAsync(dto.RefreshToken));

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        if (UserId is null) return Unauthorized();
        return Result(await auth.GetMeAsync(UserId));
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        if (UserId is null) return Unauthorized();
        return Result(await auth.ChangePasswordAsync(UserId, dto));
    }
}
