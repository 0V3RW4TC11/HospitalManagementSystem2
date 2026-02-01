using Dtos.Doctor;
using MediatR;

namespace Commands.Doctor
{
    public record UpdateDoctorCommand(
        DoctorDto Dto,
        IEnumerable<Guid> SpecializationIds,
        Guid Id) : IRequest;
}
