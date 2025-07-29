using Domain.Entities;

namespace Domain.Repositories;

public interface IPatientRepository
{
    Task<Patient?> FindByIdAsync(Guid id);
    
    void Add(Patient patient);
    
    void Remove(Patient patient);
}