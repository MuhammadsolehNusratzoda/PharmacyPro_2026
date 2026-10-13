using PharmacyPro.Application.Common;
using PharmacyPro.Application.DTOs;
using PharmacyPro.Application.Filters;
using PharmacyPro.Application.Interfaces;
using PharmacyPro.Application.Responses;
using PharmacyPro.Domain.Entities;

namespace PharmacyPro.Application.Services;

public class SaleService(
    IRepository<Sale> repo,
    ICurrentUserService currentUser,
    ReportCache reportCache) : ISaleService
{
    public async Task<PagedResponse<SaleDto>> GetAllAsync(SaleFilter filter)
    {
        var q = repo.Query();

        if (filter.From.HasValue) q = q.Where(s => s.SaleDate >= filter.From.Value.ToUtc());
        if (filter.To.HasValue) q = q.Where(s => s.SaleDate < filter.To.Value.EndExclusive());
        if (filter.Method.HasValue) q = q.Where(s => s.Method == filter.Method.Value);
        if (filter.MinAmount.HasValue) q = q.Where(s => s.Amount >= filter.MinAmount.Value);
        if (filter.MaxAmount.HasValue) q = q.Where(s => s.Amount <= filter.MaxAmount.Value);

        return await q.OrderByDescending(s => s.SaleDate).ToPagedAsync(filter, s => s.ToDto());
    }

    public async Task<Response<SaleDto>> GetByIdAsync(int id)
    {
        var sale = await repo.GetByIdAsync(id);
        return sale is null
            ? Response<SaleDto>.Fail("Фурӯш ёфт нашуд", 404)
            : Response<SaleDto>.Success(sale.ToDto());
    }

    public async Task<Response<SaleDto>> CreateAsync(CreateSaleDto dto)
    {
        if (currentUser.UserId is null) return Response<SaleDto>.Fail("Ворид нашудаед", 401);

        var sale = await repo.AddAsync(new Sale
        {
            Amount = dto.Amount,
            SaleDate = dto.SaleDate.ToUtc(),
            Method = dto.Method,
            Note = dto.Note,
            CreatedByUserId = currentUser.UserId
        });

        reportCache.Invalidate(sale.SaleDate);
        return Response<SaleDto>.Success(sale.ToDto(), "Фурӯш сабт шуд", 201);
    }

    public async Task<Response<SaleDto>> UpdateAsync(int id, UpdateSaleDto dto)
    {
        var sale = await repo.GetByIdAsync(id);
        if (sale is null) return Response<SaleDto>.Fail("Фурӯш ёфт нашуд", 404);

        reportCache.Invalidate(sale.SaleDate); // санаи кӯҳна
        sale.Amount = dto.Amount;
        sale.SaleDate = dto.SaleDate.ToUtc();
        sale.Method = dto.Method;
        sale.Note = dto.Note;

        await repo.UpdateAsync(sale);
        reportCache.Invalidate(sale.SaleDate); // санаи нав
        return Response<SaleDto>.Success(sale.ToDto(), "Фурӯш тағйир ёфт");
    }

    public async Task<Response<bool>> DeleteAsync(int id)
    {
        var sale = await repo.GetByIdAsync(id);
        if (sale is null) return Response<bool>.Fail("Фурӯш ёфт нашуд", 404);

        await repo.DeleteAsync(sale);
        reportCache.Invalidate(sale.SaleDate);
        return Response<bool>.Success(true, "Фурӯш нест карда шуд");
    }
}