using Microsoft.EntityFrameworkCore;
using PharmacyPro.Application.Common;
using PharmacyPro.Application.DTOs;
using PharmacyPro.Application.Filters;
using PharmacyPro.Application.Interfaces;
using PharmacyPro.Application.Responses;
using PharmacyPro.Domain.Entities;

namespace PharmacyPro.Application.Services;

public class FirmService(IRepository<Firm> repo) : IFirmService
{
    public async Task<PagedResponse<FirmDto>> GetAllAsync(FirmFilter filter)
    {
        var q = repo.Query();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var s = filter.Search.Trim().ToLower();
            q = q.Where(f => f.Name.ToLower().Contains(s));
        }
        if (filter.IsActive.HasValue)
            q = q.Where(f => f.IsActive == filter.IsActive.Value);

        return await q.OrderBy(f => f.Name).ToPagedAsync(filter, f => f.ToDto());
    }

    public async Task<Response<FirmDto>> GetByIdAsync(int id)
    {
        var firm = await repo.GetByIdAsync(id);
        return firm is null
            ? Response<FirmDto>.Fail("Фирма ёфт нашуд", 404)
            : Response<FirmDto>.Success(firm.ToDto());
    }

    public async Task<Response<FirmDto>> CreateAsync(CreateFirmDto dto)
    {
        var name = dto.Name.Trim();
        if (await repo.Query().AnyAsync(f => f.Name.ToLower() == name.ToLower()))
            return Response<FirmDto>.Fail("Фирма бо ин ном аллакай мавҷуд аст", 409);

        var firm = await repo.AddAsync(new Firm { Name = name, Phone = dto.Phone, Address = dto.Address });
        return Response<FirmDto>.Success(firm.ToDto(), "Фирма илова шуд", 201);
    }

    public async Task<Response<FirmDto>> UpdateAsync(int id, UpdateFirmDto dto)
    {
        var firm = await repo.GetByIdAsync(id);
        if (firm is null) return Response<FirmDto>.Fail("Фирма ёфт нашуд", 404);

        firm.Name = dto.Name.Trim();
        firm.Phone = dto.Phone;
        firm.Address = dto.Address;
        firm.IsActive = dto.IsActive;

        await repo.UpdateAsync(firm);
        return Response<FirmDto>.Success(firm.ToDto(), "Фирма тағйир ёфт");
    }

    public async Task<Response<bool>> DeleteAsync(int id)
    {
        var firm = await repo.GetByIdAsync(id);
        if (firm is null) return Response<bool>.Fail("Фирма ёфт нашуд", 404);

        await repo.DeleteAsync(firm);
        return Response<bool>.Success(true, "Фирма нест карда шуд");
    }
}