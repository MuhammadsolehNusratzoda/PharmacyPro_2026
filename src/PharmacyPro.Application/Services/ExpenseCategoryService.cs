using Microsoft.EntityFrameworkCore;
using PharmacyPro.Application.Common;
using PharmacyPro.Application.DTOs;
using PharmacyPro.Application.Interfaces;
using PharmacyPro.Application.Responses;
using PharmacyPro.Domain.Entities;

namespace PharmacyPro.Application.Services;

public class ExpenseCategoryService(IRepository<ExpenseCategory> repo) : IExpenseCategoryService
{
    public async Task<Response<List<ExpenseCategoryDto>>> GetAllAsync()
    {
        var list = await repo.Query().OrderBy(c => c.Name).ToListAsync();
        return Response<List<ExpenseCategoryDto>>.Success(list.Select(c => c.ToDto()).ToList());
    }

    public async Task<Response<ExpenseCategoryDto>> CreateAsync(CreateExpenseCategoryDto dto)
    {
        var name = dto.Name.Trim();
        if (await repo.Query().AnyAsync(c => c.Name.ToLower() == name.ToLower()))
            return Response<ExpenseCategoryDto>.Fail("Категория аллакай мавҷуд аст", 409);

        var cat = await repo.AddAsync(new ExpenseCategory { Name = name });
        return Response<ExpenseCategoryDto>.Success(cat.ToDto(), "Категория илова шуд", 201);
    }

    public async Task<Response<ExpenseCategoryDto>> UpdateAsync(int id, CreateExpenseCategoryDto dto)
    {
        var cat = await repo.GetByIdAsync(id);
        if (cat is null) return Response<ExpenseCategoryDto>.Fail("Категория ёфт нашуд", 404);

        cat.Name = dto.Name.Trim();
        await repo.UpdateAsync(cat);
        return Response<ExpenseCategoryDto>.Success(cat.ToDto(), "Категория тағйир ёфт");
    }

    public async Task<Response<bool>> DeleteAsync(int id)
    {
        var cat = await repo.GetByIdAsync(id);
        if (cat is null) return Response<bool>.Fail("Категория ёфт нашуд", 404);

        await repo.DeleteAsync(cat);
        return Response<bool>.Success(true, "Категория нест карда шуд");
    }
}