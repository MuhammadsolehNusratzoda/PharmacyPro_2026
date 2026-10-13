using Microsoft.EntityFrameworkCore;
using PharmacyPro.Application.Common;
using PharmacyPro.Application.DTOs;
using PharmacyPro.Application.Filters;
using PharmacyPro.Application.Interfaces;
using PharmacyPro.Application.Responses;
using PharmacyPro.Domain.Entities;

namespace PharmacyPro.Application.Services;

public class ExpenseService(
    IRepository<Expense> repo,
    IRepository<ExpenseCategory> categoryRepo,
    ICurrentUserService currentUser,
    ReportCache reportCache) : IExpenseService
{
    public async Task<PagedResponse<ExpenseDto>> GetAllAsync(ExpenseFilter filter)
    {
        var q = repo.Query().Include(e => e.Category).AsQueryable();

        if (filter.CategoryId.HasValue) q = q.Where(e => e.CategoryId == filter.CategoryId.Value);
        if (filter.From.HasValue) q = q.Where(e => e.ExpenseDate >= filter.From.Value.ToUtc());
        if (filter.To.HasValue) q = q.Where(e => e.ExpenseDate < filter.To.Value.EndExclusive());

        return await q.OrderByDescending(e => e.ExpenseDate).ToPagedAsync(filter, e => e.ToDto());
    }

    public async Task<Response<ExpenseDto>> GetByIdAsync(int id)
    {
        var e = await repo.Query().Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id);
        return e is null
            ? Response<ExpenseDto>.Fail("Хароҷот ёфт нашуд", 404)
            : Response<ExpenseDto>.Success(e.ToDto());
    }

    public async Task<Response<ExpenseDto>> CreateAsync(CreateExpenseDto dto)
    {
        if (currentUser.UserId is null) return Response<ExpenseDto>.Fail("Ворид нашудаед", 401);

        var cat = await categoryRepo.GetByIdAsync(dto.CategoryId);
        if (cat is null) return Response<ExpenseDto>.Fail("Категория ёфт нашуд", 404);

        var e = await repo.AddAsync(new Expense
        {
            CategoryId = dto.CategoryId,
            Amount = dto.Amount,
            ExpenseDate = dto.ExpenseDate.ToUtc(),
            Note = dto.Note,
            CreatedByUserId = currentUser.UserId
        });
        e.Category = cat;

        reportCache.Invalidate(e.ExpenseDate);
        return Response<ExpenseDto>.Success(e.ToDto(), "Хароҷот сабт шуд", 201);
    }

    public async Task<Response<ExpenseDto>> UpdateAsync(int id, UpdateExpenseDto dto)
    {
        var e = await repo.Query().Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id);
        if (e is null) return Response<ExpenseDto>.Fail("Хароҷот ёфт нашуд", 404);

        var cat = await categoryRepo.GetByIdAsync(dto.CategoryId);
        if (cat is null) return Response<ExpenseDto>.Fail("Категория ёфт нашуд", 404);

        reportCache.Invalidate(e.ExpenseDate);
        e.CategoryId = dto.CategoryId;
        e.Category = cat;
        e.Amount = dto.Amount;
        e.ExpenseDate = dto.ExpenseDate.ToUtc();
        e.Note = dto.Note;

        await repo.UpdateAsync(e);
        reportCache.Invalidate(e.ExpenseDate);
        return Response<ExpenseDto>.Success(e.ToDto(), "Хароҷот тағйир ёфт");
    }

    public async Task<Response<bool>> DeleteAsync(int id)
    {
        var e = await repo.GetByIdAsync(id);
        if (e is null) return Response<bool>.Fail("Хароҷот ёфт нашуд", 404);

        await repo.DeleteAsync(e);
        reportCache.Invalidate(e.ExpenseDate);
        return Response<bool>.Success(true, "Хароҷот нест карда шуд");
    }
}