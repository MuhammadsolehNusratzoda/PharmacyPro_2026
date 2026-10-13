using Microsoft.EntityFrameworkCore;
using PharmacyPro.Application.Filters;
using PharmacyPro.Application.Responses;

namespace PharmacyPro.Application.Common;

public static class QueryExtensions
{
    public static async Task<PagedResponse<TDto>> ToPagedAsync<TEntity, TDto>(
        this IQueryable<TEntity> query, PaginationFilter filter, Func<TEntity, TDto> map)
    {
        var total = await query.CountAsync();
        var items = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return PagedResponse<TDto>.Create(items.Select(map).ToList(), total, filter.PageNumber, filter.PageSize);
    }
}