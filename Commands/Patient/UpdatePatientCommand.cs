using MediatR;

namespace Commands.Patient
{
    public record UpdatePatientCommand(Guid Id, PatientData Data) : IRequest;
}