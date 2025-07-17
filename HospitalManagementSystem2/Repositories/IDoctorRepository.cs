using HospitalManagementSystem2.Models.Entities;

namespace HospitalManagementSystem2.Repositories;

public interface IDoctorRepository
{
    IQueryable<Doctor> Doctors { get; }
    Task AddAsync(Doctor doctor);
    Task<Doctor?> FindByIdAsync(Guid id);
    Task UpdateAsync(Doctor doctor);
    Task RemoveAsync(Doctor doctor);
}