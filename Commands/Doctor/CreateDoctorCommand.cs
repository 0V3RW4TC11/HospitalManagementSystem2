using MediatR;

namespace Commands.Doctor
{
    public record CreateDoctorCommand(
        string FirstName,
        string LastName,
        string Gender,
        string Address,
        string Phone,
        string Email,
        DateOnly? DateOfBirth,
        IEnumerable<Guid> SpecializationIds) : IRequest;
}