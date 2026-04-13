using Domain.Entities;

namespace Domain.Repositories;

public interface ISpecializationRepository
{
    void Add(Specialization specialization);

    Task<IEnumerable<Specialization>> ContainsNameAsync(string name);

    Task<bool> ExistsAsync(Guid id);

    Task<bool> ExistsAsync(string name);

    Task<Specialization?> FindByIdAsync(Guid id);

    Task<IEnumerable<Specialization>> GetAllAsync();

    Task<IEnumerable<Specialization>> GetFromIdsAsync(IEnumerable<Guid> ids);

    Task<bool> IsExistingIdsAsync(IEnumerable<Guid> ids);

    void Remove(Specialization specialization);

    Task<IEnumerable<Specialization>> SpecializationsPaged(int pageNumber, int pageSize);

    Task<int> TotalCount();
}