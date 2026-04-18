using Ardalis.Specification;

namespace Specifications.Specialization
{
    public class SpecializationIdByNameSpec : SingleResultSpecification<Entities.Specialization, Guid>
    {
        public SpecializationIdByNameSpec(string name)
        {
            var normalizedName = name.ToLower();
            Query
                .Where(s => s.Name.ToLower() == normalizedName)
                .Select(s => s.Id);
        }
    }
}