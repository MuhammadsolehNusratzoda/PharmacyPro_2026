using Microsoft.EntityFrameworkCore;
using PharmacyPro.Application.Interfaces;
using PharmacyPro.Domain.Common;
using PharmacyPro.Infrastructure.Data;

namespace PharmacyPro.Infrastructure.Repositories;

public class Repository<T>(AppDbContext db) : IRepository<T> where T : BaseEntity
{
    private readonly DbSet<T> _set = db.Set<T>();

    public IQueryable<T> Query() => _set.AsQueryable();

    public Task<T?> GetByIdAsync(int id) => _set.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<T> AddAsync(T entity)
    {
        await _set.AddAsync(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _set.Update(entity);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        entity.IsDeleted = true; // soft delete
        entity.UpdatedAt = DateTime.UtcNow;
        _set.Update(entity);
        await db.SaveChangesAsync();
    }
}