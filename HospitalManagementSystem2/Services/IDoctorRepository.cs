using HospitalManagementSystem2.Models.Entities;

namespace HospitalManagementSystem2.Services
{
    public interface IDoctorRepository
    {
        Task CreateAsync(Doctor doctor, string password);
        Task<Doctor?> FindByIdAsync(Guid id);
        Task UpdateAsync(Doctor doctor);
        Task DeleteAsync(Doctor doctor);
        IQueryable<Doctor> Doctors { get; }
    }
}
