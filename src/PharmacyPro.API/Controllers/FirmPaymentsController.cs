using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyPro.Application.DTOs;
using PharmacyPro.Application.Filters;
using PharmacyPro.Application.Interfaces;
using PharmacyPro.Domain.Constants;

namespace PharmacyPro.API.Controllers;

[Route("api/firm-payments")]
[Authorize(Roles = Roles.Admin)]
public class FirmPaymentsController(IFirmPaymentService service) : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] FirmPaymentFilter filter) =>
        Result(await service.GetAllAsync(filter));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) =>
        Result(await service.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> Create(CreateFirmPaymentDto dto) =>
        Result(await service.CreateAsync(dto));

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateFirmPaymentDto dto) =>
        Result(await service.UpdateAsync(id, dto));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) =>
        Result(await service.DeleteAsync(id));
}
