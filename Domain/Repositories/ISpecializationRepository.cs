using Domain.Entities;

namespace Domain.Repositories;

public interface ISpecializationRepository
{
    Task<IEnumerable<Specialization>> GetAllAsync();
    
    Task<IEnumerable<Specialization>> GetFromIdsAsync(IEnumerable<Guid> ids);
    
    Task<Specialization?> FindByIdAsync(Guid id);

    Task<bool> ExistsAsync(Guid id);
    
    Task<bool> ExistsAsync(string name);
    
    void Add(Specialization specialization);
    
    void Remove(Specialization specialization);
}