using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyPro.Application.Interfaces;
using PharmacyPro.Domain.Constants;

namespace PharmacyPro.API.Controllers;

[Route("api/reports")]
[Authorize(Roles = Roles.Admin)]
public class ReportsController(IReportService service) : BaseApiController
{
    // GET /api/reports/monthly?year=2026&month=10
    [HttpGet("monthly")]
    public async Task<IActionResult> Monthly(
        [FromQuery, Range(2000, 2100)] int year,
        [FromQuery, Range(1, 12)] int month) =>
        Result(await service.GetMonthlyReportAsync(year, month));

    // GET /api/reports/weekly?year=2026&month=10
    [HttpGet("weekly")]
    public async Task<IActionResult> Weekly(
        [FromQuery, Range(2000, 2100)] int year,
        [FromQuery, Range(1, 12)] int month) =>
        Result(await service.GetWeeklySalesAsync(year, month));
}
