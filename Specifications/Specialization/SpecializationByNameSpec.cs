using Ardalis.Specification;

namespace Specifications.Specialization
{
    public class SpecializationByNameSpec : Specification<Domain.Entities.Specialization>
    {
        public SpecializationByNameSpec(string name)
        {
            var normalizedName = name.ToLower();
            Query.Where(s => s.Name.ToLower() == normalizedName);
        }
    }
}