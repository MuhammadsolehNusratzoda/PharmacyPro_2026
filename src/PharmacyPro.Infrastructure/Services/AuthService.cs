using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PharmacyPro.Application.DTOs;
using PharmacyPro.Application.Interfaces;
using PharmacyPro.Application.Responses;
using PharmacyPro.Domain.Constants;
using PharmacyPro.Infrastructure.Data;
using PharmacyPro.Infrastructure.Identity;

namespace PharmacyPro.Infrastructure.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    AppDbContext db,
    ITokenService tokenService,
    IOptions<JwtSettings> options) : IAuthService
{
    private readonly JwtSettings _jwt = options.Value;
    private static readonly string[] AllowedRoles = [Roles.Admin, Roles.Pharmacist];

    public async Task<Response<UserInfoDto>> RegisterAsync(RegisterDto dto)
    {
        if (!AllowedRoles.Contains(dto.Role))
            return Response<UserInfoDto>.Fail("Нақш нодуруст аст");

        var email = dto.Email.Trim().ToLowerInvariant();
        if (await userManager.FindByEmailAsync(email) is not null)
            return Response<UserInfoDto>.Fail("Корбар бо ин email мавҷуд аст", 409);

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = dto.FullName.Trim(),
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return Response<UserInfoDto>.Fail("Сабт анҷом нашуд", 400,
                result.Errors.Select(e => e.Description).ToList());

        await userManager.AddToRoleAsync(user, dto.Role);
        return Response<UserInfoDto>.Success(ToUserInfo(user, [dto.Role]), "Корбар сабт шуд", 201);
    }

    public async Task<Response<AuthResponseDto>> LoginAsync(LoginDto dto)
    {
        const string invalid = "Email ё парол нодуруст аст";

        var user = await userManager.FindByEmailAsync(dto.Email.Trim());
        if (user is null || !user.IsActive)
            return Response<AuthResponseDto>.Fail(invalid, 401);

        if (await userManager.IsLockedOutAsync(user))
            return Response<AuthResponseDto>.Fail("Ҳисоб муваққатан баста шуд. Баъдтар кӯшиш кунед", 423);

        if (!await userManager.CheckPasswordAsync(user, dto.Password))
        {
            await userManager.AccessFailedAsync(user);
            return Response<AuthResponseDto>.Fail(invalid, 401);
        }

        await userManager.ResetAccessFailedCountAsync(user);

        var (auth, _) = await IssueTokensAsync(user);
        await db.SaveChangesAsync();
        return Response<AuthResponseDto>.Success(auth, "Хуш омадед");
    }

    public async Task<Response<AuthResponseDto>> RefreshAsync(string refreshToken)
    {
        var hash = tokenService.Hash(refreshToken);
        var stored = await db.RefreshTokens.Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == hash);

        if (stored is null)
            return Response<AuthResponseDto>.Fail("Token нодуруст аст", 401);

        // Токени аллакай истифодашуда дубора омад: эҳтимоли дуздӣ, ҳамаро бекор мекунем
        if (stored.RevokedAt is not null)
        {
            await RevokeAllAsync(stored.UserId);
            return Response<AuthResponseDto>.Fail("Token аллакай истифода шудааст. Дубора ворид шавед", 401);
        }

        if (stored.ExpiresAt <= DateTime.UtcNow || !stored.User.IsActive)
            return Response<AuthResponseDto>.Fail("Token ба охир расидааст", 401);

        var (auth, newHash) = await IssueTokensAsync(stored.User);
        stored.RevokedAt = DateTime.UtcNow;
        stored.ReplacedByTokenHash = newHash;
        await db.SaveChangesAsync();

        return Response<AuthResponseDto>.Success(auth);
    }

    public async Task<Response<bool>> LogoutAsync(string refreshToken)
    {
        var hash = tokenService.Hash(refreshToken);
        var stored = await db.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hash);

        if (stored is not null && stored.RevokedAt is null)
        {
            stored.RevokedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }
        return Response<bool>.Success(true, "Баромадед");
    }

    public async Task<Response<UserInfoDto>> GetMeAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null) return Response<UserInfoDto>.Fail("Корбар ёфт нашуд", 404);

        var roles = await userManager.GetRolesAsync(user);
        return Response<UserInfoDto>.Success(ToUserInfo(user, roles));
    }

    public async Task<Response<bool>> ChangePasswordAsync(string userId, ChangePasswordDto dto)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null) return Response<bool>.Fail("Корбар ёфт нашуд", 404);

        var result = await userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
        if (!result.Succeeded)
            return Response<bool>.Fail("Парол иваз нашуд", 400,
                result.Errors.Select(e => e.Description).ToList());

        await RevokeAllAsync(userId); // ҳамаи session-ҳо баста мешаванд
        return Response<bool>.Success(true, "Парол иваз шуд. Дубора ворид шавед");
    }

    // ---------- helpers ----------

    private async Task<(AuthResponseDto Auth, string RefreshHash)> IssueTokensAsync(ApplicationUser user)
    {
        var roles = await userManager.GetRolesAsync(user);
        var accessToken = tokenService.CreateAccessToken(user, roles);
        var (refresh, hash) = tokenService.GenerateRefreshToken();

        db.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = hash,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwt.RefreshTokenDays)
        });

        var auth = new AuthResponseDto(
            accessToken,
            refresh,
            DateTime.UtcNow.AddMinutes(_jwt.AccessTokenMinutes),
            ToUserInfo(user, roles));

        return (auth, hash);
    }

    private Task RevokeAllAsync(string userId) =>
        db.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.RevokedAt, DateTime.UtcNow));

    private static UserInfoDto ToUserInfo(ApplicationUser u, IList<string> roles) =>
        new(u.Id, u.FullName, u.Email!, roles);
}