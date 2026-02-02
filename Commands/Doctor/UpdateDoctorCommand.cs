namespace Commands.Doctor
{
    public record UpdateDoctorCommand(Guid Id) : DoctorBaseCommand;
}