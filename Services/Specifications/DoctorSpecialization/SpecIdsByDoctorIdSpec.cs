using Ardalis.Specification;

namespace Services.Specifications.DoctorSpecialization
{
    internal class SpecIdsByDoctorIdSpec : Specification<Domain.Entities.DoctorSpecialization, Guid>
    {
        public SpecIdsByDoctorIdSpec(Guid doctorId)
        {
            Query
                .Select(ds => ds.SpecializationId)
                .Where(ds => ds.DoctorId == doctorId);
        }
    }
}
