using Domain.Entities;

namespace Domain.Repositories;

public interface IDoctorSpecializationRepository
{
    Task<IEnumerable<Guid>> GetSpecIdsByDoctorIdAsync(Guid doctorId);
    
    Task UpdateAsync(Guid doctorId, IEnumerable<Guid> specializationIds);
}