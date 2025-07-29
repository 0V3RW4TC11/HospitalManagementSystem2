using System.Linq.Expressions;
using Domain.Entities;

namespace Domain.Repositories;

public interface IAdminRepository
{
    Task<Admin?> FindByIdAsync(Guid id);

    Task<bool> ExistsAsync(Expression<Func<Admin, bool>> predicate);
    
    void Add(Admin admin);
    
    void Remove(Admin admin);
}