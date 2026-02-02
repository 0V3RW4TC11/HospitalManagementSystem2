namespace Commands.Specialization
{
    public record UpdateSpecializationCommand(Guid Id) : CreateSpecializationCommand;
}