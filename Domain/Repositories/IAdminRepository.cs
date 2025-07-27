using Domain.Entities;

namespace Domain.Repositories;

public interface IAdminRepository
{
    Task<Admin?> FindByIdAsync(Guid id);
    
    void Add(Admin admin);
    
    void Remove(Admin admin);
}