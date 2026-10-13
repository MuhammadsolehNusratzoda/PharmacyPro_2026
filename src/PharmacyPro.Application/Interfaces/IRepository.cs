using PharmacyPro.Domain.Common;

namespace PharmacyPro.Application.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    IQueryable<T> Query();
    Task<T?> GetByIdAsync(int id);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity); // soft delete
}