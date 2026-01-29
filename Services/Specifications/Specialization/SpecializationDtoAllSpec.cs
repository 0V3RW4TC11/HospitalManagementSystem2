using Ardalis.Specification;
using Mapster;
using Services.Dtos.Specialization;

namespace Services.Specifications.Specialization
{
    internal class SpecializationDtoAllSpec : Specification<Domain.Entities.Specialization, SpecializationDto>
    {
        public SpecializationDtoAllSpec()
        {
            Query.Select(s => s.Adapt<SpecializationDto>());
        }
    }
}
