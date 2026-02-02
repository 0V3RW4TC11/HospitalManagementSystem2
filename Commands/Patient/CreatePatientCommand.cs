namespace Commands.Patient
{
    public record CreatePatientCommand(string Password) : PatientBaseCommand;
}