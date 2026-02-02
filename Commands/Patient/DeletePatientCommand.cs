using MediatR;

namespace Commands.Patient
{
    public record DeletePatientCommand(Guid Id) : IRequest;
}