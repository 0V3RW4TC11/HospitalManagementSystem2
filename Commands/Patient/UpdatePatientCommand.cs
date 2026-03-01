using MediatR;

namespace Commands.Patient
{
    public record UpdatePatientCommand(
        Guid Id,
        string? Title,
        string FirstName,
        string? LastName,
        string Gender,
        string? Address,
        string? Phone,
        string Email,
        Constants.BloodType BloodType,
        DateOnly DateOfBirth) : IRequest;
}