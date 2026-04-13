using Ardalis.Specification;

namespace Specifications.DoctorSpecialization
{
    public class SpecializationIdsByDoctorIdSpec : Specification<Domain.Entities.DoctorSpecialization, Guid>
    {
        public SpecializationIdsByDoctorIdSpec(Guid id)
        {
            Query
                .Where(ds => ds.DoctorId == id)
                .Select(ds => ds.SpecializationId);
        }
    }
}