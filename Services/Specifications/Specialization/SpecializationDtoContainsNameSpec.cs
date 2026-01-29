using Ardalis.Specification;
using Mapster;
using Services.Dtos.Specialization;

namespace Services.Specifications.Specialization
{
    internal class SpecializationDtoContainsNameSpec : Specification<Domain.Entities.Specialization, SpecializationDto>
    {
        public SpecializationDtoContainsNameSpec(string name)
        {
            Query
                .Select(s => s.Adapt<SpecializationDto>())
                .Search(s => s.Name, name);
        }
    }
}
