using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyPro.Application.DTOs;
using PharmacyPro.Application.Filters;
using PharmacyPro.Application.Interfaces;
using PharmacyPro.Domain.Constants;

namespace PharmacyPro.API.Controllers;

[Route("api/firms")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Pharmacist}")]
public class FirmsController(IFirmService service) : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] FirmFilter filter) =>
        Result(await service.GetAllAsync(filter));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) =>
        Result(await service.GetByIdAsync(id));

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create(CreateFirmDto dto) =>
        Result(await service.CreateAsync(dto));

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(int id, UpdateFirmDto dto) =>
        Result(await service.UpdateAsync(id, dto));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(int id) =>
        Result(await service.DeleteAsync(id));
}
