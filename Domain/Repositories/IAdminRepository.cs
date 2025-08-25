using System.Linq.Expressions;
using Domain.Entities;

namespace Domain.Repositories;

public interface IAdminRepository
{
    Task<IEnumerable<(Guid Id, string FirstName, string? LastName, string Email)>> GetAdminListAsync();

    Task<Admin?> FindByIdAsync(Guid id);

    Task<bool> ExistsAsync(Expression<Func<Admin, bool>> predicate);
    
    void Add(Admin admin);
    
    void Remove(Admin admin);
}