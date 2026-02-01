using Dtos.Patient;
using MediatR;

namespace Commands.Patient
{
    public record CreatePatientCommand(PatientDto Dto, string Password) : IRequest;
}
