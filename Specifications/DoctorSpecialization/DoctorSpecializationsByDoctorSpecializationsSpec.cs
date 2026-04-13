using Ardalis.Specification;

namespace Specifications.DoctorSpecialization
{
    public class DoctorSpecializationsByDoctorSpecializationsSpec : Specification<Domain.Entities.DoctorSpecialization>
    {
        public DoctorSpecializationsByDoctorSpecializationsSpec(IEnumerable<Domain.Entities.DoctorSpecialization> doctorSpecs)
        {
            Query.Where(ds => doctorSpecs.Any(x =>
                ds.DoctorId == x.DoctorId &&
                ds.SpecializationId == x.SpecializationId));
        }
    }
}
