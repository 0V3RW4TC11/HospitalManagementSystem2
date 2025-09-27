using Domain.Entities;

namespace Domain.Repositories;

public interface ISpecializationRepository
{
    Task<IEnumerable<Specialization>> GetAllAsync();
    
    Task<IEnumerable<Specialization>> GetFromIdsAsync(IEnumerable<Guid> ids);

    Task<IEnumerable<Specialization>> ContainsNameAsync(string name);

    Task<Specialization?> FindByIdAsync(Guid id);

    Task<bool> ExistsAsync(Guid id);
    
    Task<bool> ExistsAsync(string name);

    Task<bool> IsExistingIdsAsync(IEnumerable<Guid> ids);

    void Add(Specialization specialization);
    
    void Remove(Specialization specialization);
}