using Microsoft.AspNetCore.Mvc;
using PharmacyPro.Application.Responses;

namespace PharmacyPro.API.Controllers;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected string? UserId => User.FindFirst("sub")?.Value;

    protected IActionResult Result<T>(Response<T> response) =>
        StatusCode(response.StatusCode, response);
}
