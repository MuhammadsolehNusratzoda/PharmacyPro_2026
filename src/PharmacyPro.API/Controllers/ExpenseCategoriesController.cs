using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyPro.Application.DTOs;
using PharmacyPro.Application.Interfaces;
using PharmacyPro.Domain.Constants;

namespace PharmacyPro.API.Controllers;

[Route("api/expense-categories")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Pharmacist}")]
public class ExpenseCategoriesController(IExpenseCategoryService service) : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Result(await service.GetAllAsync());

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create(CreateExpenseCategoryDto dto) =>
        Result(await service.CreateAsync(dto));

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(int id, CreateExpenseCategoryDto dto) =>
        Result(await service.UpdateAsync(id, dto));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(int id) =>
        Result(await service.DeleteAsync(id));
}
