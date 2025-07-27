using Domain.Entities;

namespace Domain.Repositories;

public interface IDoctorRepository
{
    Task<Doctor?> FindByIdAsync(Guid id);
    
    void Add(Doctor doctor);
    
    void Remove(Doctor doctor);
}