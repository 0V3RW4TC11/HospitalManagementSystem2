using Ardalis.Specification;

namespace Specifications.DoctorSpecialization
{
    public class DoctorSpecializationsByDoctorSpecializationsSpec : Specification<Entities.DoctorSpecialization>
    {
        public DoctorSpecializationsByDoctorSpecializationsSpec(IEnumerable<Entities.DoctorSpecialization> doctorSpecs)
        {
            Query.Where(ds => doctorSpecs.Any(x =>
                ds.DoctorId == x.DoctorId &&
                ds.SpecializationId == x.SpecializationId));
        }
    }
}
