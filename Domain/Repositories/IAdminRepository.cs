using System.Linq.Expressions;
using Domain.Entities;

namespace Domain.Repositories;

public interface IAdminRepository
{
    Task<IEnumerable<Admin>> GetAdmins(int pageNumber, int pageSize);

    Task<int> GetTotalCount();

    Task<Admin?> FindByIdAsync(Guid id);

    Task<bool> ExistsAsync(Expression<Func<Admin, bool>> predicate);
    
    void Add(Admin admin);
    
    void Remove(Admin admin);
}