using Microsoft.AspNetCore.Http;
using PharmacyPro.Application.Interfaces;

namespace PharmacyPro.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
{
    public string? UserId => accessor.HttpContext?.User.FindFirst("sub")?.Value;
}