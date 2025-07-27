using Domain.Entities;

namespace Domain.Repositories;

public interface ISpecializationRepository
{
    Task<IEnumerable<Specialization>> GetAllAsync();
    
    Task<IEnumerable<Specialization>> GetFromIdSetAsync(ISet<Guid> ids);
    
    Task<Specialization?> FindByIdAsync(Guid id);

    Task<bool> ContainsSetAsync(ISet<Guid> ids, out Guid notFound);
    
    void Add(Specialization specialization);
    
    void Remove(Specialization specialization);
}