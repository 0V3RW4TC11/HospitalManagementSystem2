using Dtos.Doctor;
using MediatR;

namespace Commands.Doctor
{
    public record CreateDoctorCommand(
        DoctorDto Dto, 
        IEnumerable<Guid> SpecializationIds,
        string Password) : IRequest;
}
