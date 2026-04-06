using MediatR;

namespace Commands.Doctor
{
    public record UpdateDoctorCommand(Guid Id, DoctorData Data) : IRequest;
}