using MediatR;

namespace Commands.Doctor
{
    public record DeleteDoctorCommand(Guid Id) : IRequest;
}