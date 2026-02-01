using Dtos.Patient;
using MediatR;

namespace Commands.Patient.UpdatePatient
{
    public record UpdatePatientCommand(PatientDto Dto, Guid Id) : IRequest;
}
