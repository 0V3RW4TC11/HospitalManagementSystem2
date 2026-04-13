using System.Linq.Expressions;
using Domain.Entities;

namespace Domain.Repositories;

public interface IPatientRepository
{
    Task<IEnumerable<Patient>> Patients(int pageNumber, int pageSize);

    Task<int> GetTotalCount();

    Task<Patient?> FindByIdAsync(Guid id);
    
    Task<bool> ExistsAsync(Expression<Func<Patient, bool>> predicate);
    
    void Add(Patient patient);
    
    void Remove(Patient patient);
}