using HospitalManagementSystem2.Models.Entities;

namespace HospitalManagementSystem2.Repositories;

public interface IDoctorSpecializationRepository
{
    IQueryable<DoctorSpecialization> DoctorSpecializations { get; }
    
    Task AddRangeAsync(IEnumerable<DoctorSpecialization> doctorSpecialization);
    
    Task RemoveRangeAsync(IEnumerable<DoctorSpecialization> doctorSpecialization);
}