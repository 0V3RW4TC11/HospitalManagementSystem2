namespace Commands.Patient.UpdatePatient
{
    public record UpdatePatientCommand(Guid Id) : PatientBaseCommand;
}