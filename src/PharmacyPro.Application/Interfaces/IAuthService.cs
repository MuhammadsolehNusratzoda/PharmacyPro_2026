using PharmacyPro.Application.DTOs;
using PharmacyPro.Application.Responses;

namespace PharmacyPro.Application.Interfaces;

public interface IAuthService
{
    Task<Response<UserInfoDto>> RegisterAsync(RegisterDto dto);
    Task<Response<AuthResponseDto>> LoginAsync(LoginDto dto);
    Task<Response<AuthResponseDto>> RefreshAsync(string refreshToken);
    Task<Response<bool>> LogoutAsync(string refreshToken);
    Task<Response<UserInfoDto>> GetMeAsync(string userId);
    Task<Response<bool>> ChangePasswordAsync(string userId, ChangePasswordDto dto);
}