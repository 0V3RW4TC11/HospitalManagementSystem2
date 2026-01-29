using Ardalis.Specification;

namespace Services.Specifications.Specialization
{
    internal class SpecializationByNameSpec : Specification<Domain.Entities.Specialization>
    {
        public SpecializationByNameSpec(string name)
        {
            Query.Where(s => s.Name.ToLower() == name.ToLower());
        }
    }
}
