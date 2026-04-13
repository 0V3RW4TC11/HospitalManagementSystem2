using Domain.Entities;

namespace Domain.Repositories;

public interface IDoctorSpecializationRepository
{
    Task<IEnumerable<Guid>> GetSpecIdsByDoctorIdAsync(Guid doctorId);

    void AddRange(IEnumerable<DoctorSpecialization> doctorSpecializations);

    void RemoveRange(IEnumerable<DoctorSpecialization> doctorSpecializations);
}