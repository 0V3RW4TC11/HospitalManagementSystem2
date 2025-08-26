using System.Linq.Expressions;
using System.Numerics;
using Domain.Entities;

namespace Domain.Repositories;

public interface IDoctorRepository
{
    Task<IEnumerable<Doctor>> Doctors(int pageNumber, int pageSize);

    Task<int> GetTotalCount();

    Task<Doctor?> FindByIdAsync(Guid id);
    
    Task<bool> ExistsAsync(Expression<Func<Doctor, bool>> predicate);
    
    void Add(Doctor doctor);
    
    void Remove(Doctor doctor);
}