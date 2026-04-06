using MediatR;

namespace Commands.Patient
{
    public record CreatePatientCommand(PatientData Data, string Password) : IRequest;
}