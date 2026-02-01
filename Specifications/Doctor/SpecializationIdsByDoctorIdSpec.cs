using Ardalis.Specification;
using Domain.Entities;

namespace Specifications.Doctor
{
    public class SpecializationIdsByDoctorIdSpec : Specification<DoctorSpecialization, Guid>
    {
        public SpecializationIdsByDoctorIdSpec(Guid id)
        {
            Query
                .Where(ds => ds.DoctorId == id)
                .Select(ds => ds.SpecializationId);
        }
    }
}
