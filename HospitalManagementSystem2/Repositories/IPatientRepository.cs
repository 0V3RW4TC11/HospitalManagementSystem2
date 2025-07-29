using HospitalManagementSystem2.Models.Entities;

namespace HospitalManagementSystem2.Repositories;

public interface IPatientRepository
{
    IQueryable<Patient> Patients { get; }
    
    Task AddAsync(Patient patient);
    
    Task UpdateAsync(Patient patient);
    
    Task RemoveAsync(Patient patient);
}