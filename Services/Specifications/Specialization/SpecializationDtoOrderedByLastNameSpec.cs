using Ardalis.Specification;
using Mapster;
using Services.Dtos.Specialization;

namespace Services.Specifications.Specialization
{
    internal class SpecializationDtoOrderedByLastNameSpec : Specification<Domain.Entities.Specialization, SpecializationDto>
    {
        public SpecializationDtoOrderedByLastNameSpec()
        {
            Query
                .Select(s => s.Adapt<SpecializationDto>())
                .OrderBy(s => s.Name);
        }
    }
}
