using MediatR;

namespace Commands.Doctor
{
    public record CreateDoctorCommand(DoctorData Data, string Password) : IRequest;
}