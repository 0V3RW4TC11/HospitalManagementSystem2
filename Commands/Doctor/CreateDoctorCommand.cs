namespace Commands.Doctor
{
    public record CreateDoctorCommand(string Password) : DoctorBaseCommand;
}