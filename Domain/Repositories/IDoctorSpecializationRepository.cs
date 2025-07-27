using Domain.Entities;

namespace Domain.Repositories;

public interface IDoctorSpecializationRepository
{
    Task<ISet<Guid>> GetSpecIdsByDoctorIdAsync(Guid doctorId);
    
    void Update(Guid doctorId, IEnumerable<Guid> specializationIds);
}