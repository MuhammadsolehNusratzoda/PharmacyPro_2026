using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyPro.Application.DTOs;
using PharmacyPro.Application.Filters;
using PharmacyPro.Application.Interfaces;
using PharmacyPro.Domain.Constants;

namespace PharmacyPro.API.Controllers;

[Route("api/expenses")]
[Authorize(Roles = Roles.Admin)]
public class ExpensesController(IExpenseService service) : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ExpenseFilter filter) =>
        Result(await service.GetAllAsync(filter));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) =>
        Result(await service.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> Create(CreateExpenseDto dto) =>
        Result(await service.CreateAsync(dto));

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateExpenseDto dto) =>
        Result(await service.UpdateAsync(id, dto));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) =>
        Result(await service.DeleteAsync(id));
}
