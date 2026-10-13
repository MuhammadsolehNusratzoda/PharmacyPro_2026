using Microsoft.EntityFrameworkCore;
using PharmacyPro.Application.Common;
using PharmacyPro.Application.DTOs;
using PharmacyPro.Application.Filters;
using PharmacyPro.Application.Interfaces;
using PharmacyPro.Application.Responses;
using PharmacyPro.Domain.Entities;

namespace PharmacyPro.Application.Services;

public class FirmPaymentService(
    IRepository<FirmPayment> repo,
    IRepository<Firm> firmRepo,
    ReportCache reportCache) : IFirmPaymentService
{
    public async Task<PagedResponse<FirmPaymentDto>> GetAllAsync(FirmPaymentFilter filter)
    {
        var q = repo.Query().Include(p => p.Firm).AsQueryable();

        if (filter.FirmId.HasValue) q = q.Where(p => p.FirmId == filter.FirmId.Value);
        if (filter.From.HasValue) q = q.Where(p => p.PaymentDate >= filter.From.Value.ToUtc());
        if (filter.To.HasValue) q = q.Where(p => p.PaymentDate < filter.To.Value.EndExclusive());

        return await q.OrderByDescending(p => p.PaymentDate).ToPagedAsync(filter, p => p.ToDto());
    }

    public async Task<Response<FirmPaymentDto>> GetByIdAsync(int id)
    {
        var p = await repo.Query().Include(x => x.Firm).FirstOrDefaultAsync(x => x.Id == id);
        return p is null
            ? Response<FirmPaymentDto>.Fail("Пардохт ёфт нашуд", 404)
            : Response<FirmPaymentDto>.Success(p.ToDto());
    }

    public async Task<Response<FirmPaymentDto>> CreateAsync(CreateFirmPaymentDto dto)
    {
        var firm = await firmRepo.GetByIdAsync(dto.FirmId);
        if (firm is null) return Response<FirmPaymentDto>.Fail("Фирма ёфт нашуд", 404);

        var p = await repo.AddAsync(new FirmPayment
        {
            FirmId = dto.FirmId,
            Amount = dto.Amount,
            PaymentDate = dto.PaymentDate.ToUtc(),
            Method = dto.Method,
            Note = dto.Note
        });
        p.Firm = firm;

        reportCache.Invalidate(p.PaymentDate);
        return Response<FirmPaymentDto>.Success(p.ToDto(), "Пардохт сабт шуд", 201);
    }

    public async Task<Response<FirmPaymentDto>> UpdateAsync(int id, UpdateFirmPaymentDto dto)
    {
        var p = await repo.Query().Include(x => x.Firm).FirstOrDefaultAsync(x => x.Id == id);
        if (p is null) return Response<FirmPaymentDto>.Fail("Пардохт ёфт нашуд", 404);

        var firm = await firmRepo.GetByIdAsync(dto.FirmId);
        if (firm is null) return Response<FirmPaymentDto>.Fail("Фирма ёфт нашуд", 404);

        reportCache.Invalidate(p.PaymentDate);
        p.FirmId = dto.FirmId;
        p.Firm = firm;
        p.Amount = dto.Amount;
        p.PaymentDate = dto.PaymentDate.ToUtc();
        p.Method = dto.Method;
        p.Note = dto.Note;

        await repo.UpdateAsync(p);
        reportCache.Invalidate(p.PaymentDate);
        return Response<FirmPaymentDto>.Success(p.ToDto(), "Пардохт тағйир ёфт");
    }

    public async Task<Response<bool>> DeleteAsync(int id)
    {
        var p = await repo.GetByIdAsync(id);
        if (p is null) return Response<bool>.Fail("Пардохт ёфт нашуд", 404);

        await repo.DeleteAsync(p);
        reportCache.Invalidate(p.PaymentDate);
        return Response<bool>.Success(true, "Пардохт нест карда шуд");
    }
}